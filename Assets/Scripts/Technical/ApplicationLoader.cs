using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class ApplicationLoader : SingletonMonobehaviour<ApplicationLoader> {

    [SerializeField]
    private GameObject gameManager;

    CameraController cameraController;
    ModelObjectFrameUpdater modelObjectFrameUpdater;
    MapGenerator mapGenerator;
    MapRenderer mapRenderer;
    EconomyView economyView;
    CreepManager creepManager;
    InterfaceController interfaceController;
    GameStateManager gameStateManager;
    AbilityCooldownView abilityCooldownView;

    private void Awake()
    {
        CDebug.InitialiseDebugChannels();

        CDebug.Log(CDebug.applicationLoading, "Application Loader Awake");

        modelObjectFrameUpdater = gameManager.GetComponent<ModelObjectFrameUpdater>();
        Assert.IsTrue(modelObjectFrameUpdater != null);

        mapGenerator = gameManager.GetComponent<MapGenerator>(); //loads map model
        Assert.IsTrue(mapGenerator != null);

        mapRenderer = gameManager.GetComponent<MapRenderer>();
        Assert.IsTrue(mapRenderer != null);

        cameraController = gameManager.GetComponent<CameraController>();
        Assert.IsTrue(cameraController != null);

        economyView = gameManager.GetComponent<EconomyView>(); //loads economy model
        Assert.IsTrue(economyView != null);

        creepManager = gameManager.GetComponent<CreepManager>();
        Assert.IsTrue(creepManager != null);

        interfaceController = gameManager.GetComponent<InterfaceController>();
        Assert.IsTrue(interfaceController != null);

        gameStateManager = gameManager.GetComponent<GameStateManager>();
        Assert.IsTrue(gameStateManager != null);

        abilityCooldownView = gameManager.GetComponent<AbilityCooldownView>();
        Assert.IsTrue(abilityCooldownView != null);

        CDebug.Log(CDebug.applicationLoading, "Application Loader Finished Awake");
    }

    private void Start()
    {
        CDebug.Log(CDebug.applicationLoading, "Application Loader Start");

        modelObjectFrameUpdater.enabled = true;
        mapGenerator.enabled = true;
        mapRenderer.enabled = true;
        cameraController.enabled = true;
        economyView.enabled = true;
        creepManager.enabled = true;
        interfaceController.enabled = true;
        gameStateManager.enabled = true;
        abilityCooldownView.enabled = true;

        CDebug.Log(CDebug.applicationLoading, "Application Loader Finished Start");
    }
}
