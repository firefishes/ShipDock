using ShipDock;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Demo2 : ShipDockAppComponent
{
    public override void EnterGameHandler()
    {
        base.EnterGameHandler();

        InitECS();

        //¼ÓÔØ³¡¾°
        Scenes scenes = new();
        scenes.OnSceneLoaded += (s, sceneManagement) =>
        {
            scenes.OnSceneLoaded = default;
            StartGame();
        };
        scenes.LoadAndClearAnotherScene("GameStart_Demo2");
    }

    private void InitECS()
    {
        ECS.Instance.MemorySizeInBytes = 140;
        ECS.Instance.InitECS();
        ECS.Instance.InitChunkGroup<Movement>(1, 0, 10000);//, 0, 0, 64);
        ECS.Instance.CreateComponent<MovementComponent, Movement>(Consts.TENON_TYPE_MOVEMENT);
        ECS.Instance.AddSystem<MovementSystem>();

        ECS.Instance.AllEntitas.BuildEntiy(1, new int[] { Consts.TENON_TYPE_MOVEMENT });
    }

    private void StartGame()
    {
        //for (int i = 0; i < 10; i++)
        //{
        //    ECS.Instance.AllEntitas.CreateEntiyByType(1);
        //}
    }
}
