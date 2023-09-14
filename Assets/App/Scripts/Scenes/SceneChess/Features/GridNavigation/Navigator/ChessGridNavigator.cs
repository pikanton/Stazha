using System;
using System.Collections.Generic;
using App.Scripts.Scenes.SceneChess.Features.ChessField.GridMatrix;
using App.Scripts.Scenes.SceneChess.Features.ChessField.Piece;
using App.Scripts.Scenes.SceneChess.Features.ChessField.Types;
using UnityEngine;

namespace App.Scripts.Scenes.SceneChess.Features.GridNavigation.Navigator
{
    public class ChessGridNavigator : IChessGridNavigator
    {

        public List<Vector2Int> FindPath(ChessUnitType unit, Vector2Int from, Vector2Int to, ChessGrid grid)
        {
            List<Vector2Int> openList = new();
            HashSet<Vector2Int> closedList = new();
            Dictionary<Vector2Int, Vector2Int> cameFrom = new();
            Dictionary<Vector2Int, int> costs = new();

            openList.Add(from);
            costs[from] = 0;

            while (openList.Count > 0)
            {
                Vector2Int current = FindLowestCost(openList, costs);

                if (current == to)
                {
                    return ReconstructPath(cameFrom, current);
                }

                openList.Remove(current);
                closedList.Add(current);

                List<Vector2Int> neighbors = GetNeighbors(current, unit, grid);

                foreach (var neighbor in neighbors)
                {
                    if (closedList.Contains(neighbor))
                        continue;

                    int nextCost = costs[current] + 1; // 1 - это стоимость одного хода

                    if (!openList.Contains(neighbor) || nextCost < costs[neighbor])
                    {
                        cameFrom[neighbor] = current;
                        costs[neighbor] = nextCost;

                        if (!openList.Contains(neighbor))
                            openList.Add(neighbor);
                    }
                }
            }
            return null;
        }
        private Vector2Int FindLowestCost(List<Vector2Int> openList, Dictionary<Vector2Int, int> cost)
        {
            int lowestCost = int.MaxValue;
            Vector2Int lowestCostCell = Vector2Int.zero;

            foreach (var cell in openList)
            {
                if (cost.ContainsKey(cell) && cost[cell] < lowestCost)
                {
                    lowestCost = cost[cell];
                    lowestCostCell = cell;
                }
            }
            return lowestCostCell;
        }
        private List<Vector2Int> GetNeighbors(Vector2Int cell,ChessUnitType unit, ChessGrid grid)
        {
            switch (unit)
            {
                case ChessUnitType.Pon:
                    return GetPonNeighbors(cell, grid);
                case ChessUnitType.Bishop:
                    return GetBishopNeighbors(cell, grid);
                case ChessUnitType.Knight:
                    return GetKnightNeighbors(cell, grid);
                case ChessUnitType.Rook:
                    return GetRookNeighbors(cell, grid);
                case ChessUnitType.King:
                    return GetKingNeighbors(cell, grid);
                case ChessUnitType.Queen:
                    return GetQueenNeighbors(cell, grid);
                default:
                    return null;
            }

        }
        private List<Vector2Int> GetPonNeighbors(Vector2Int cell, ChessGrid grid)
        {
            List<Vector2Int> neighbors = new();

            for (int i = -1; i <= 1; i += 2)
            {
                if (i == 0)
                    continue;

                int newY = cell.y + i;

                if (IsPositionValid(new Vector2Int(cell.x, newY), grid))
                {
                    neighbors.Add(new Vector2Int(cell.x, newY));
                }
            }
            return neighbors;
        }
        private List<Vector2Int> GetBishopNeighbors(Vector2Int cell, ChessGrid grid)
        {
            List<Vector2Int> neighbors = new();

            for (int i = -1; i <= 1; i += 2)
            {
                for (int j = -1; j <= 1; j += 2)
                {
                    int newX = cell.x + i;
                    int newY = cell.y + j;

                    while (IsPositionValid(new Vector2Int(newX, newY), grid))
                    {
                        neighbors.Add(new Vector2Int(newX, newY));
                        newX += i;
                        newY += j;
                    }
                }
            }
            return neighbors;
        }
        private List<Vector2Int> GetKnightNeighbors(Vector2Int cell, ChessGrid grid)
        {
            List<Vector2Int> neighbors = new();

            for (int i = -2; i <= 2; i++)
            {
                for (int j = -2; j <= 2; j++)
                {
                    if (i == 0 || j == 0 || Math.Abs(i) - Math.Abs(j) == 0)
                        continue;

                    int newX = cell.x + i;
                    int newY = cell.y + j;

                    if (IsPositionValid(new Vector2Int(newX, newY), grid))
                    {
                        neighbors.Add(new Vector2Int(newX, newY));
                    }
                }
            }
            return neighbors;
        }
        private List<Vector2Int> GetRookNeighbors(Vector2Int cell, ChessGrid grid)
        {
            List<Vector2Int> neighbors = new();

            for (int i = -1; i <= 1; i += 2)
            {
                if (i == 0)
                    continue;

                int newX = cell.x + i;

                while (IsPositionValid(new Vector2Int(newX, cell.y), grid))
                {
                    neighbors.Add(new Vector2Int(newX, cell.y));
                    newX += i;
                }
            }

            for (int j = -1; j <= 1; j += 2)
            {
                int newY = cell.y + j;

                while (IsPositionValid(new Vector2Int(cell.x, newY), grid))
                {
                    neighbors.Add(new Vector2Int(cell.x, newY));
                    newY += j;
                }
            }
            return neighbors;
        }
        private List<Vector2Int> GetKingNeighbors(Vector2Int cell, ChessGrid grid)
        {
            List<Vector2Int> neighbors = new();

            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (i == 0 && j == 0)
                        continue;

                    int newX = cell.x + i;
                    int newY = cell.y + j;

                    if (IsPositionValid(new Vector2Int(newX, newY), grid))
                    {
                        neighbors.Add(new Vector2Int(newX, newY));
                    }
                }
            }
            return neighbors;
        }
        private List<Vector2Int> GetQueenNeighbors(Vector2Int cell, ChessGrid grid)
        {
            List<Vector2Int> neighbors = new();

            neighbors.AddRange(GetBishopNeighbors(cell, grid));
            neighbors.AddRange(GetRookNeighbors(cell, grid));

            return neighbors;
        }
        private List<Vector2Int> ReconstructPath(Dictionary<Vector2Int, Vector2Int> cameFrom, Vector2Int current)
        {
            List<Vector2Int> path = new();
            while (cameFrom.ContainsKey(current))
            {
                path.Insert(0, current); //0 - что бы ходы были последовательны
                current = cameFrom[current];
            }
            path.Insert(0, current);
            return path;
        }
        private bool IsPositionValid(Vector2Int position, ChessGrid grid)
        {
            Vector2Int size = grid.Size;
            if (position.x >= 0 && position.x < size.x && position.y >= 0 && 
                    position.y < size.y && grid.Get(position) == null)
                return true;
            else return false;
        }
    }
}