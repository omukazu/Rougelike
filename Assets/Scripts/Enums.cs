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
        type = 8,
        category = 9
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

    enum RoomType : int
    {
        deleted = -1,
        normal = 0,
        hidden = 1,
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

    // for Spawn.cs
    enum Enemies : int
    {
        bat = 0,
    }

    enum ItemType : int
    {
        weapon = 0,
        armor = 1,
        miscellaneous = 2,
        accessory = 3,
    }

    enum Weapon : int
    {
        woodenStick = 0,
        blade = 1,
        blazingSword = 2,
        bow = 3,
    }

    enum Armor : int
    {
        cloth = 0,
        leather = 1,
        steal = 2,
        diamond = 3,
    }

    enum Miscellaneous : int
    {
        wrapper = 0,
        herb = 1,
        strongPortion = 2,
        scarecrow = 3,
        scroll = 4,
    }

    enum Accessory : int
    {
        ring = 0,
        necklace = 1,
        sandals = 2
    }

    // for action.cs
    enum State
    {
        None,
        Open,
        Closed
    }
}