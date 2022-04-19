using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : CharacterStats
{
    [HideInInspector]
    public PlayerData_SO playerData;

    #region Read from Ddata_SO

    public int CurrentExp
    {
        get { return playerData.currentExp; }
        set { playerData.currentExp = value; }
    }
    public int NextLevelExp
    {
        get { return playerData.nextLevelExp; }
        set { playerData.nextLevelExp = value; }
    }
    public int MaxLevel
    {
        get { return playerData.maxLevel; }
        set { playerData.maxLevel = value; }
    }
    public int CurrentLevel
    {
        get { return playerData.currentLevel; }
        set { playerData.currentLevel = value; }
    }

    #endregion

    protected override void Awake()
    {
        base.Awake();
        playerData = characterData as PlayerData_SO;
    }

    public override void TakeDamage(int damage, bool isCritical = false)
    {
        base.TakeDamage(damage, isCritical);
    }

    public void UpdateExp(int point)
    {
        playerData.currentExp += point;

        if (playerData.currentExp >= playerData.nextLevelExp)
            LevelUp();
    }

    private void LevelUp()
    {
        playerData.currentLevel = Mathf.Min(playerData.currentLevel + 1, playerData.maxLevel);
        playerData.nextLevelExp = (int)(playerData.nextLevelExp * playerData.levelBuff);

        playerData.maxHealth = (int)(playerData.maxHealth * playerData.healthBuff);
        playerData.currentHealth = playerData.maxHealth;

        Debug.Log("LEVEL UP! " + playerData.currentHealth + " MaxHealth: " + playerData.maxHealth);
    }
}
