using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Linq;

namespace Rougelike
{
    public class CharacterControl : MonoBehaviour
    {
        private GameObject pointer;

        private static bool playerTurn;

        private Dictionary<Coordinates, GameObject> empty;
        public List<Action> actionSequence;

        void Start()
        {
            var p = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            p.x = (int)Math.Round(p.x, 0, MidpointRounding.AwayFromZero);
            p.y = (int)Math.Round(p.y, 0, MidpointRounding.AwayFromZero);
            p.z = 0;
            pointer = Instantiate(MasterData.pointer, p, Quaternion.identity);

            empty = new Dictionary<Coordinates, GameObject>();
            actionSequence = new List<Action>();

            playerTurn = true;
        }

        void Update()
        {
            var p = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            p.x = (int)Math.Round(p.x, 0, MidpointRounding.AwayFromZero);
            p.y = (int)Math.Round(p.y, 0, MidpointRounding.AwayFromZero);
            p.z = 0;
            pointer.transform.position = p;

            if (playerTurn)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    playerTurn = false;
                    StartCoroutine(Waiting());
                }
            }
        }

        float CalculateTime()
        {
            var time = 0f;
            var delays = false;
            Action action;
            for (int t = 0; t < actionSequence.Count; t++)
            {
                action = actionSequence[t];
                switch (action.P)
                {
                    case ActionPattern.step:
                        delays = true;
                        if (t > 0 && actionSequence[t - 1].P == ActionPattern.swap)
                        {
                            time += 0.1f;
                        }
                        if (t < actionSequence.Count - 1 && actionSequence[t + 1].S == action.S)
                        {
                            time += 0.1f;
                        }
                        break;
                    case ActionPattern.swap:
                        delays = true;
                        if (t < actionSequence.Count - 1 && actionSequence[t + 1].S == action.S)
                        {
                            time += 0.1f;
                        }
                        break;
                    case ActionPattern.attack:
                        time += 0.3f;
                        break;
                    default:
                        break;
                }
            }

            if (delays || actionSequence.Count == 0)
            {
                time += 0.1f;
            }
            return time;
        }

        IEnumerator Waiting()
        {
            _Wait();
            float time = CalculateTime();
            StartCoroutine(ActEnemies());
            yield return new WaitForSecondsRealtime(time);
            playerTurn = true;
        }

        IEnumerator ActEnemies()
        {
            Action action;
            for (int t = 0; t < actionSequence.Count; t++)
            {
                action = actionSequence[t];
                switch (action.P)
                {
                    case ActionPattern.step:
                        StartCoroutine(Steping(action.S, action.Sc, action.Tc));
                        if (t < actionSequence.Count - 1 && (actionSequence[t + 1].S == action.S || actionSequence[t + 1].P == ActionPattern.swap))
                        {
                            yield return new WaitForSecondsRealtime(0.1f);
                        }
                        break;
                    case ActionPattern.swap:
                        StartCoroutine(Swaping(action.S, action.Sc, action.T, action.Tc));
                        yield return new WaitForSecondsRealtime(0.1f);
                        break;
                    case ActionPattern.attack:
                        // StartCoroutine(MessageControl.Text(action.Sc, action.Message, Color.red));
                        yield return new WaitForSecondsRealtime(0.3f);
                        break;
                    default:
                        break;
                }
            }
        }

        IEnumerator Steping(GameObject source, Coordinates before, Coordinates after)
        {
            var position = new Vector3(before.X, before.Y, 0);
            float trigger = 0;

            while (trigger < 1)
            {
                trigger += 5 * Time.deltaTime;
                position.x += 5 * (after.X - before.X) * Time.deltaTime;
                position.y += 5 * (after.Y - before.Y) * Time.deltaTime;
                source.transform.position = position;
                yield return null;
            }

            position.x = after.X;
            position.y = after.Y;
            source.transform.position = position;
        }

        IEnumerator Swaping(GameObject source, Coordinates sourceP, GameObject target, Coordinates targetP)
        {
            var sourcePosition = new Vector3(sourceP.X, sourceP.Y, 0);
            var targetPosition = new Vector3(targetP.X, targetP.Y, 0);
            float trigger = 0;

            while (trigger < 1)
            {
                trigger += 10 * Time.deltaTime;
                sourcePosition.x += 10 * (targetP.X - sourceP.X) * Time.deltaTime;
                sourcePosition.y += 10 * (targetP.Y - sourceP.Y) * Time.deltaTime;
                targetPosition.x += 10 * (sourceP.X - targetP.X) * Time.deltaTime;
                targetPosition.y += 10 * (sourceP.Y - targetP.Y) * Time.deltaTime;
                source.transform.position = sourcePosition;
                target.transform.position = targetPosition;
                yield return null;
            }

            sourcePosition.x = targetP.X;
            sourcePosition.y = targetP.Y;
            targetPosition.x = sourceP.X;
            targetPosition.y = sourceP.Y;
            source.transform.position = sourcePosition;
            target.transform.position = targetPosition;
        }

        Coordinates TransformMouseInput(Vector3 mousePosition)
        {
            mousePosition.z = -1.0f;
            var pos = Camera.main.ScreenToWorldPoint(mousePosition);
            int x = (int)Math.Round(pos.x, 0, MidpointRounding.AwayFromZero);
            int y = (int)Math.Round(pos.y, 0, MidpointRounding.AwayFromZero);
            return new Coordinates(x, y);
        }

        (List<GameObject>, List<Enemy>) CalculateDistance()
        {
            int count = Spawn.enemies.Count;
            var goal = Spawn.pCache.p;
            var distance = new SortedDictionary<int, int>();

            for (int n = 0; n < count; n++)
            {
                var pathCount = AstarAlgorithm.GetPath(Spawn.eCaches[n].p, goal, empty, false).Count;
                distance.Add(pathCount * count + n, n);
            }

            var order = new int[count];
            distance.Values.CopyTo(order, 0);

            var sortedEnemies = new List<GameObject>();
            var sortedECaches = new List<Enemy>();
            for (int i = 0; i < order.Length; i++)
            {
                sortedEnemies.Add(Spawn.enemies[order[i]]);
                sortedECaches.Add(Spawn.eCaches[order[i]]);
            }
            return (sortedEnemies, sortedECaches);
        }

        GameObject SetTarget(Enemy sourceComponent)
        {
            var obstacles = sourceComponent.obstacles;
            if (obstacles.Count > 0)
            {
                // priority ... scarecrow -> other enemies' target object -> player -> null
                var scarecrows = obstacles.Values.Where(value => Spawn.scarecrows.Contains(value));
                if (scarecrows.Count() > 0)
                {
                    return scarecrows.First();
                }

                var sourceP = sourceComponent.p;
                var goal = new Coordinates(0, 0);
                for (int j = -2; j <= 2; j++)
                {
                    for (int i = -2; i <= 2; i++)
                    {
                        if (i == 0 && j == 0)
                        {
                            continue;
                        }

                        goal.X = sourceP.X + i;
                        goal.Y = sourceP.Y + j;
                        if (!Spawn.characters.ContainsKey(goal) || Spawn.characters[goal] == Spawn.player)
                        {
                            continue;
                        }

                        var targetComponent = Spawn.characters[goal].GetComponent<Enemy>();  // if there are some characters, they are always enemies
                        if (targetComponent.chase && (AstarAlgorithm.GetPath(sourceP, goal, empty, false).Count <= 2))
                        {
                            return targetComponent.targetObject;
                        }
                    }
                }

                if (obstacles.Values.Contains(Spawn.player))
                {
                    return Spawn.player;
                }
            }
            return null;
        }

        bool EnemyDetected(Coordinates p)
        {
            var hash = Spawn.enemies.ToHashSet();
            return Spawn.characters.ContainsKey(p) && hash.Contains(Spawn.characters[p]);
        }

        void _Wait()
        {
            GameObject source, target;
            Enemy sourceComponent;
            (List<GameObject> sortedEnemies, List<Enemy> sortedECaches) = CalculateDistance();
            actionSequence.Clear();

            for (int k = 0; k < Spawn.enemies.Count; k++)
            {
                source = sortedEnemies[k];
                sourceComponent = sortedECaches[k];
                for (int i = 0; i < (int)sortedECaches[k].speed; i++)
                {
                    sourceComponent.obstacles = ObstacleSetter.GetObstacles(sourceComponent.p, null, false);
                    target = SetTarget(sourceComponent);  // player or scarecrow
                    if (target == null)
                    {
                        sourceComponent.targetObject = null;
                        sourceComponent.chase = false;
                        _Seek(source, sourceComponent);  // does not find or gives up chasing a target
                    }
                    else  // perceive a target
                    {
                        _Reset(sourceComponent);
                        sourceComponent.targetObject = target;
                        sourceComponent.chase = true;
                        var targetComponent = target.GetComponent<Character>();
                        var targetP = targetComponent.p;
                        bool attack = Attacks(source, sourceComponent, target, targetP);
                        if (attack)
                        {
                            _Attack(sourceComponent, targetComponent);
                        }
                        else
                        {
                            _Chase(source, sourceComponent, target, targetComponent);
                        }
                    }
                }
            }

            for (int i = 0; i < Spawn.eCaches.Count; i++)
            {
                Spawn.eCaches[i].swap = false;
            }
        }

        void _Seek(GameObject source, Enemy sourceComponent)
        {
            var sourceP = sourceComponent.p;

            if (sourceComponent.patient <= 0)
            {
                sourceComponent.patient = 5;
                sourceComponent.path.Clear();
            }

            if (sourceComponent.path.Count == 0)  // set path
            {
                _SetDestination(sourceComponent);
            }

            if (!sourceComponent.swap)
            {
                var step = sourceComponent.path[0];
                if (!Spawn.characters.ContainsKey(step))
                {
                    _Reset(sourceComponent);
                    actionSequence.Add(new Step(ActionPattern.step, source, sourceP, step));
                    sourceComponent.path.RemoveAt(0);
                    _Synchronize(source, sourceComponent, sourceP, step);
                }
                else
                {
                    if (EnemyDetected(step))
                    {
                        var targetComponent = Spawn.characters[step].GetComponent<Enemy>();
                        sourceComponent.collisionDetected = true;
                        sourceComponent.collisionObject = Spawn.characters[step];
                        if (targetComponent.chase)
                        {
                            sourceComponent.chase = true;
                            sourceComponent.patient = 5;
                        }
                        else if (targetComponent.collisionDetected && targetComponent.collisionObject == source && !targetComponent.swap)
                        {
                            _Reset(sourceComponent);
                            _Reset(targetComponent);
                            targetComponent.swap = true;
                            actionSequence.Add(new Swap(ActionPattern.swap, source, sourceP, Spawn.characters[step], step));
                            sourceComponent.path.RemoveAt(0);
                            targetComponent.path.RemoveAt(0);
                            _Swap(source, sourceComponent, sourceP, Spawn.characters[step], targetComponent, step);
                        }
                        else if (targetComponent.collisionDetected && targetComponent.collisionObject != source)
                        {
                            sourceComponent.patient -= 1;
                        }
                    }
                }
            }
        }

        void _SetDestination(Enemy sourceComponent)
        {
            var destination = new Coordinates(0, 0);
            do
            {
                if (sourceComponent.doorNumbers.Count == 0)
                {
                    int[] doorNumbers = Enumerable.Range(1, Dungeon.nDoor).ToArray();
                    sourceComponent.doorNumbers = doorNumbers.OrderBy(a => Guid.NewGuid()).ToList();
                    if (sourceComponent.doorNumbers[0] == sourceComponent.targetDoorNumber)
                    {
                        sourceComponent.doorNumbers.RemoveAt(0);
                        sourceComponent.doorNumbers.Add(sourceComponent.targetDoorNumber);
                    }
                }

                destination = Dungeon.doors[sourceComponent.doorNumbers[0]];
                sourceComponent.targetDoorNumber = sourceComponent.doorNumbers.Pop();
                sourceComponent.path = AstarAlgorithm.GetPath(sourceComponent.p, destination, empty, seek: true);
            } while (sourceComponent.path.Count == 0);
        }

        void _Reset(Enemy component)
        {
            component.collisionDetected = false;
            component.collisionObject = null;
            component.patient = 5;
        }

        void _Synchronize(GameObject source, Character sourceComponent, Coordinates before, Coordinates after)
        {
            sourceComponent.p = after;
            Spawn.characters.Remove(before);
            Spawn.characters[after] = source;
        }

        void _Swap(GameObject source, Enemy sourceComponent, Coordinates sourceP, GameObject target, Enemy targetComponent, Coordinates targetP)
        {
            sourceComponent.p = targetP;
            targetComponent.p = sourceP;

            Spawn.characters.Remove(targetP);
            Spawn.characters[targetP] = source;
            Spawn.characters.Remove(sourceP);
            Spawn.characters[sourceP] = target;
        }

        void _Chase(GameObject source, Enemy sourceComponent, GameObject target, Character targetComponent)
        {
            var sourceP = sourceComponent.p;
            var targetP = targetComponent.p;
            var shortest = AstarAlgorithm.GetPath(sourceP, targetP, empty, seek: false);
            var obstacles = ObstacleSetter.GetObstacles(sourceP, target: target, player: false);
            var detour = AstarAlgorithm.GetPath(sourceP, targetP, obstacles, seek: false);

            sourceComponent.path.Clear();

            if (shortest.Count > 0 && !Spawn.characters.ContainsKey(shortest[0]))
            {
                actionSequence.Add(new Step(ActionPattern.step, source, sourceP, shortest[0]));
                _Synchronize(source, sourceComponent, sourceP, shortest[0]);
            }
            else if (detour.Count > 0 && !Spawn.characters.ContainsKey(detour[0]) && detour.Count < shortest.Count + ((Dungeon.unitW + Dungeon.unitH) / 2))
            {
                actionSequence.Add(new Step(ActionPattern.step, source, sourceP, detour[0]));
                _Synchronize(source, sourceComponent, sourceP, detour[0]);
            }
            else if (detour.Count == 0 || (detour.Count > 0 && detour.Count >= shortest.Count + ((Dungeon.unitW + Dungeon.unitH) / 2)))
            {
                _Approch(source, sourceComponent, sourceP, targetP);
            }
            else { }  // Do not move, but still continue to chase
        }

        void _Approch(GameObject source, Character sourceComponent, Coordinates sourceP, Coordinates targetP)
        {
            var directions = new int[8, 2] { { 1, 0 }, { 0, 1 }, { -1, 0 }, { 0, -1 }, { 1, 1 }, { 1, -1 }, { -1, 1 }, { -1, -1 } };
            int distance = (int)Math.Pow(targetP.X - sourceP.X, 2) + (int)Math.Pow(targetP.Y - sourceP.Y, 2);
            int minDistance = distance;
            int index = -1;

            var p = new Coordinates(0, 0);
            for (int i = 0; i < 8; i++)
            {
                p.X = sourceP.X + directions[i, 0];
                p.Y = sourceP.Y + directions[i, 1];
                distance = (int)Math.Pow(targetP.X - p.X, 2) + (int)Math.Pow(targetP.Y - p.Y, 2);
                if (MasterData.nodeCandidates.Contains(Dungeon.map[p.X, p.Y]) && !Spawn.characters.ContainsKey(p) && minDistance >= distance)
                {
                    minDistance = distance;
                    index = i;
                }
            }

            if (index >= 0)
            {
                p.X = sourceP.X + directions[index, 0];
                p.Y = sourceP.Y + directions[index, 1];
                actionSequence.Add(new Step(ActionPattern.step, source, sourceP, p));
                _Synchronize(source, sourceComponent, sourceP, p);
            }
        }

        bool Attacks(GameObject source, Character sourceComponent, GameObject target, Coordinates targetP)
        {
            var sourceP = sourceComponent.p;

            if (Math.Max(Math.Abs(targetP.X - sourceP.X), Math.Abs(targetP.Y - sourceP.Y)) > sourceComponent.range)
            {
                return false;
            }
            else if (Math.Max(Math.Abs(targetP.X - sourceP.X), Math.Abs(targetP.Y - sourceP.Y)) <= 1)
            {
                return true;
            }
            else // 1 < diff <= range
            {
                var boxCollider2D = source.GetComponent<BoxCollider2D>();
                boxCollider2D.enabled = false;  // unable to detect self
                var origin = new Vector2(sourceP.X, sourceP.Y);
                var direction = new Vector2(targetP.X - sourceP.X, targetP.Y - sourceP.Y);
                var ray2D = new Ray2D(origin, direction);
                var raycastHit2D = Physics2D.Raycast(ray2D.origin, ray2D.direction);
                boxCollider2D.enabled = true;

                return raycastHit2D.collider.gameObject == target;
            }
        }

        void _Attack(Character sourceComponent, Character targetComponent)
        {

        }
    }
}