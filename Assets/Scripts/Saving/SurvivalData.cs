﻿[System.Serializable]
public class SurvivalData
{
    // Location data
    public int[,] buildings;
    public float[,] enemies;
    public int[] hotbar;
    
    // Resource data
    public int gold;
    public int essence;
    public int iridium;
    public int heat;
    public int power;

    // Progress data
    public int[] unlocked;
    public bool[] researchTechs;
    public int bossesDefeated;

    // Difficulty data
    public float enemyAmountMulti;
    public float enemyHealthMulti;
    public float enemyDamageMulti;
    public float enemySpeedMulti;
    public bool enemyOutposts;
    public bool enemyGroups;
    public bool enemyGuaridans;

    // World data
    public float goldSpawnAmount;
    public float essenceSpawnAmount;
    public float iridiumSpawnAmount;

    // Save data
    public string worldName;
    public string worldMode;
    public string worldSeed;
    public string worldVersion;
    public int worldPlaytime;

    public SurvivalData (Technology data_2, EnemySpawner data_3, Research data_4, int worldPlaytime = 0) 
    {
        // Location data
        // buildings = data_1.GetLocationData();
        // enemies = data_1.GetEnemyData();
        // hotbar = data_1.GetHotbarData();

        // Resource data
        gold = Resource.active.GetAmount(Resource.CurrencyType.Gold);
        essence = Resource.active.GetAmount(Resource.CurrencyType.Essence);
        iridium = Resource.active.GetAmount(Resource.CurrencyType.Iridium);
        heat = Resource.active.GetAmount(Resource.CurrencyType.Heat);
        power = Resource.active.GetAmount(Resource.CurrencyType.Power);
        
        // Progress data
        unlocked = data_2.GetSaveData();
        //researchTechs = data_4.GetResearchData();
        //bossesDefeated = data_3.bossesDefeated;

        // Difficulty data
        enemyAmountMulti = GameManager.enemySpawnrateModifier;
        enemyHealthMulti = GameManager.enemyHealthModifier;
        enemyDamageMulti = GameManager.enemyDamageModifier;
        enemySpeedMulti = GameManager.enemySpeedModifier;
        //enemyOutposts = GameManager.enemyOutposts;
        //enemyGroups = GameManager.enemyGroups;
        //enemyGuaridans = GameManager.enemyGuardians;

        // World data
        goldSpawnAmount = GameManager.goldSpawnModifier;
        essenceSpawnAmount = GameManager.essenceSpawnModifier;
        iridiumSpawnAmount = GameManager.iridiumSpawnModifier;

        // Save data
        worldName = GameManager.name;
        worldMode = GameManager.mode;
        worldSeed = GameManager.seed;
        worldVersion = GameManager.version;
        this.worldPlaytime = worldPlaytime;
    }
}
