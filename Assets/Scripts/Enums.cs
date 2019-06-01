using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rougelike
{
    enum Index : int
    {
        width = 0,
        height = 1,
        xLeft = 2,
        yBottom = 3,
        rightDoor = 4,
        leftDoor = 5,
        topDoor = 6,
        bottomDoor = 7,
        shape = 8,
        type = 9,
        category = 10
    }

    enum Shape : int
    {
        square = 0,
        circle = 1
    }

    enum Tile : int
    {
        end = -2,
        border = -1,
        wall = 0,
        division = 1,
        floor = 2,
        path = 3,
        door = 4,
        hFloor = 5,
        hPath = 6,
        hDoor = 7,
        hidden = 8,
    }

    enum Type : int
    {
        deleted = -1,
        normal = 0,
        hidden = 1,
    }

    enum NormalSubCategory : int
    {
        normal = 0,
        shop = 1
    }

    enum HiddenSubCategory : int
    {
        challenge = 1,
        secret = 2,
        warehouse = 3,
        garden = 4,
        spring = 5,
        tree = 6,
    }

    enum Direction : int
    {
        right = 4,
        left = 5,
        up = 6,
        down = 7,
        horizontal = 8,
        vertical = 9,
    }
}