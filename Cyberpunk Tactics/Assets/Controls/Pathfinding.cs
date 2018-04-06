using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

public class Pathfinding : MonoBehaviour {

    private List<Transform> players;
    private List<Transform> attackers;
    private HashSet<Transform> currentlySelected;
    private Dictionary<Transform, Transform> attackerMap;
    private Dictionary<Transform, Transform> seekerMap;
    private Dictionary<Transform, Transform> targetMap;
    private Dictionary<Transform, Vector3> rallyPoint;

    Grid grid;

    void Start()
    {
        players = new List<Transform>();
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Clickable"))
        {
            if(obj.GetComponent<Controller>().isUnit)
                players.Add(obj.transform);
        }
        attackers = new List<Transform>();
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Enemy"))
            attackers.Add(obj.transform);
        currentlySelected = new HashSet<Transform>();
        attackerMap = new Dictionary<Transform, Transform>();
        targetMap = new Dictionary<Transform, Transform>();
        seekerMap = new Dictionary<Transform, Transform>();
        rallyPoint = new Dictionary<Transform, Vector3>();
        grid = GetComponent<Grid>();
    }

    void Update()
    {
        foreach (Transform player in players)
        {
            if (!player)
            {
                currentlySelected.Remove(player);
                seekerMap.Remove(player);
                targetMap.Remove(player);
                rallyPoint.Remove(player);
            }
            /*else if(!currentlySelected.Contains(player) && !rallyPoint.ContainsKey(player))
            {
                float minDistance = float.MaxValue;
                foreach (Transform enemy in attackers)
                {
                    if (player && enemy && Vector3.Distance(player.position, enemy.position) < minDistance)
                    {
                        minDistance = Vector3.Distance(player.position, enemy.position);
                        targetMap[player] = enemy;
                    }
                }
            }*/
        }
        foreach (Transform enemy in attackers)
        {
            if (!enemy)
            {
                attackerMap.Remove(enemy);
            }
            /*else
            {
                float minDistance = float.MaxValue;
                foreach (Transform player in players)
                {
                    if (player && enemy && Vector3.Distance(player.position, enemy.position) < minDistance)
                    {
                        minDistance = Vector3.Distance(player.position, enemy.position);
                        attackerMap[enemy] = player;
                    }
                }
            }*/
        }
        if (Input.GetMouseButtonDown(1))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.gameObject.CompareTag("Clickable"))
                {
                    foreach (Transform seeker in currentlySelected)
                    {
                        targetMap.Remove(seeker);
                        rallyPoint.Remove(seeker);
                        seekerMap[seeker] = hit.transform;
                    }
                }
                else if (hit.transform.gameObject.CompareTag("Enemy"))
                {
                    foreach (Transform seeker in currentlySelected)
                    {
                        seekerMap.Remove(seeker);
                        rallyPoint.Remove(seeker);
                        targetMap[seeker] = hit.transform;
                    }
                }
                else if (hit.transform.gameObject.CompareTag("EnemyHead"))
                {
                    foreach (Transform seeker in currentlySelected)
                    {
                        seekerMap.Remove(seeker);
                        rallyPoint.Remove(seeker);
                        targetMap[seeker] = hit.transform.parent;
                    }
                }
                else
                {
                    foreach (Transform seeker in currentlySelected)
                    {
                        targetMap.Remove(seeker);
                        seekerMap.Remove(seeker);
                        rallyPoint[seeker] = hit.point;
                        seeker.LookAt(new Vector3(hit.point.x, seeker.position.y, hit.point.z));
                    }
                }
            }
        }
        foreach (KeyValuePair<Transform, Transform> seekerPair in targetMap)
        {
            if (seekerPair.Key && seekerPair.Value)
            {
                seekerPair.Key.LookAt(new Vector3(seekerPair.Value.position.x, seekerPair.Key.position.y, seekerPair.Value.position.z));
                if (lineOfSight(seekerPair.Key, seekerPair.Value) && Vector3.Distance(seekerPair.Key.position, seekerPair.Value.position) <= seekerPair.Key.gameObject.GetComponent<PlayerUnit>().range)
                {
                    seekerPair.Key.gameObject.GetComponent<PlayerUnit>().attack();
                    if (!seekerPair.Value.gameObject)
                    {
                        players.Remove(seekerPair.Value);
                        currentlySelected.Remove(seekerPair.Value.gameObject.transform);
                        seekerMap.Remove(seekerPair.Value.gameObject.transform);
                        targetMap.Remove(seekerPair.Value.gameObject.transform);
                        rallyPoint.Remove(seekerPair.Value.gameObject.transform);
                    }
                }
                else
                    FindPath(seekerPair.Key, seekerPair.Key.position, seekerPair.Value.position);
            }
        }
        foreach (KeyValuePair<Transform, Transform> seekerPair in attackerMap)
        {
            if (seekerPair.Key && seekerPair.Value)
            {
                seekerPair.Key.LookAt(new Vector3(seekerPair.Value.position.x, seekerPair.Key.position.y, seekerPair.Value.position.z));
                if (lineOfSight(seekerPair.Key, seekerPair.Value) && Vector3.Distance(seekerPair.Key.position, seekerPair.Value.position) <= seekerPair.Key.gameObject.GetComponent<PlayerUnit>().range)
                {
                    seekerPair.Key.gameObject.GetComponent<PlayerUnit>().attack();
                    if (!rallyPoint.ContainsKey(seekerPair.Value))
                        targetMap[seekerPair.Value] = seekerPair.Key;
                    if (!seekerPair.Value.gameObject)
                    {
                        attackers.Remove(seekerPair.Value);
                        attackerMap.Remove(seekerPair.Value.gameObject.transform);
                    }
                }
                else
                    FindPath(seekerPair.Key, seekerPair.Key.position, seekerPair.Value.position);
            }
        }
        foreach (KeyValuePair<Transform, Transform> seekerPair in seekerMap)
        {
            FindPath(seekerPair.Key, seekerPair.Key.position, seekerPair.Value.position);
            seekerPair.Key.LookAt(new Vector3(seekerPair.Value.position.x, seekerPair.Key.position.y, seekerPair.Value.position.z));
        }
        foreach (KeyValuePair<Transform, Vector3> seekerPair in rallyPoint)
        {
            if(seekerPair.Key)
                FindPath(seekerPair.Key, seekerPair.Key.position, seekerPair.Value);
        }
    }

    void FindPath(Transform seeker, Vector3 startPos, Vector3 targetPos)
    {
        Node startNode = grid.getNodeAtWorldPoint(startPos);
        Node targetNode = grid.getNodeAtWorldPoint(targetPos);

        while(targetNode.walkable == false)
        {
            List<Node> adjacent = grid.getAdjacentNodes(targetNode);
            foreach (Node node in adjacent)
            {
                targetNode = node;
                if (node.walkable)
                    break;
            }
        }
        
        Heap<Node> open = new Heap<Node>(grid.MaxSize);
        HashSet<Node> closed = new HashSet<Node>();
        open.Add(startNode);

        while (open.Count > 0)
        {
            Node current = open.RemoveFirst();
            closed.Add(current);

            if (current == targetNode)
            {
                RetracePath(seeker, startNode, targetNode);
                return;
            }

            List<Node> adjacentNodes = grid.getAdjacentNodes(current);
            foreach (Node adjacent in adjacentNodes)
            {
                if (!adjacent.walkable || closed.Contains(adjacent))
                    continue;

                int newCost = current.gCost + getDistance(current, adjacent);
                if (newCost < adjacent.gCost || !open.Contains(adjacent))
                {
                    adjacent.gCost = newCost;
                    adjacent.hCost = getDistance(adjacent, targetNode);
                    adjacent.parent = current;
                    if (!open.Contains(adjacent))
                        open.Add(adjacent);
                    else
                        open.UpdateItem(adjacent);
                }
                        
            }

        }
    }

    void RetracePath(Transform seeker, Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            if (currentNode.parent == startNode)
            {
                if(currentNode != endNode || !currentNode.characterOnNode)
                    seeker.position = Vector3.MoveTowards(seeker.position, new Vector3(currentNode.worldPosition.x, seeker.position.y, currentNode.worldPosition.z), 3 * Time.deltaTime);
                startNode.characterOnNode = false;
                grid.getNodeAtWorldPoint(seeker.position).characterOnNode = true;
            }
            currentNode = currentNode.parent;
        }
    }

    int getDistance(Node start, Node end)
    {
        int dstX = Mathf.Abs(start.gridX - end.gridX);
        int dstY = Mathf.Abs(start.gridY - end.gridY);

        if(dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }

    bool lineOfSight(Transform seeker, Transform target)
    {
        RaycastHit hit;
        if (Vector3.Angle(target.position - seeker.position, seeker.forward) <= 1.0f &&
                Physics.Linecast(seeker.position, target.position, out hit) &&
                hit.collider.transform == target)
            return true;
        return false;
    }

    public void addUnitToCurrentlySelected(Transform unit)
    {
        currentlySelected.Add(unit);
    }

    public void clearCurrentlySelected()
    {
        currentlySelected.Clear();
    }
}
