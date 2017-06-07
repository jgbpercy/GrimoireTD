using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugChannel
{
    private bool enabled;

    private string name;

    public bool Enabled
    {
        get
        {
            return enabled;
        }
    }

    public string Name
    {
        get
        {
            return name;
        }
    }

    public DebugChannel(string name, bool enabled)
    {
        this.name = name;
        this.enabled = enabled;
    }

}

public static class CDebug {

    public static bool debugChannelsEnabled = true;

    public static DebugChannel applicationLoading;
    public static DebugChannel mapGeneration;
    public static DebugChannel gameState;
    public static DebugChannel creepSpawning;
    public static DebugChannel abilityManagement;
    public static DebugChannel distanceCalculations;
    public static DebugChannel buildModeAbilities;
    public static DebugChannel combatLog;
    public static DebugChannel experienceAndFatigue;
    public static DebugChannel unitAttributes;
    public static DebugChannel structureUpgrades;
    public static DebugChannel hexEconomy;
    public static DebugChannel pathing;

    public static void InitialiseDebugChannels()
    {
        if ( !debugChannelsEnabled )
        {
            return;
        }

        Debug.Log("Debug Channels intialising");

        List<DebugChannel> channelList = new List<DebugChannel>();

        applicationLoading = new DebugChannel("Application Loading", false);
        channelList.Add(applicationLoading);

        mapGeneration = new DebugChannel("Map Generation", false);
        channelList.Add(mapGeneration);

        gameState = new DebugChannel("Game State", false);
        channelList.Add(gameState);

        creepSpawning = new DebugChannel("Creep Spawning", false);
        channelList.Add(creepSpawning);

        abilityManagement = new DebugChannel("Ability Management", false);
        channelList.Add(abilityManagement);

        distanceCalculations = new DebugChannel("Distance Calcs", false);
        channelList.Add(distanceCalculations);

        buildModeAbilities = new DebugChannel("Build Mode Abilities", false);
        channelList.Add(buildModeAbilities);

        combatLog = new DebugChannel("Combat Log", false);
        channelList.Add(combatLog);

        experienceAndFatigue = new DebugChannel("XP and Fatigue", false);
        channelList.Add(experienceAndFatigue);

        unitAttributes = new DebugChannel("Unit Attributes", false);
        channelList.Add(unitAttributes);

        structureUpgrades = new DebugChannel("Structure Upgrades", false);
        channelList.Add(structureUpgrades);

        hexEconomy = new DebugChannel("Hex Economy", false);
        channelList.Add(hexEconomy);

        pathing = new DebugChannel("Pathing", false);
        channelList.Add(pathing);

        foreach (DebugChannel debugChannel in channelList)
        {
            if (debugChannel.Enabled)
            {
                Debug.Log("Channel: " + debugChannel.Name + " enabled");
            }
        }
    }

    public static void Log(DebugChannel channel, string message)
    {
        if (channel.Enabled)
        {
            Debug.Log(channel.Name + ": " + message);
        }
    }

    public static void Log(DebugChannel channel, string message, Object context)
    {
        if (channel.Enabled)
        {
            Debug.Log(channel.Name + ": " + message, context);
        }
    }


}
