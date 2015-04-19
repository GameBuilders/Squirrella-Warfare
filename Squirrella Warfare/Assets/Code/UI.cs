using UnityEngine;
using UnityEngine.UI;
using JetBrains.Annotations;

public class UI : MonoBehaviour
{
	GameObject[] GameMenuUI;
	GameObject[] LoadoutUI;
	GameObject[] ServerMenuUI;
	
	public int primaryWeaponID = 0;
	public int secondaryWeaponID = 0;

    public bool MenuShowing { get { return MainMenuShowing || ServerMenuShowing; }}

    private bool MainMenuShowing = false;
    private bool ServerMenuShowing = false;

    [UsedImplicitly]
    void Start()
    {
		GameMenuUI = UnityEngine.GameObject.FindGameObjectsWithTag("GameMenuUI");
		LoadoutUI = UnityEngine.GameObject.FindGameObjectsWithTag("LoadoutUI");
		ServerMenuUI = UnityEngine.GameObject.FindGameObjectsWithTag("ServerMenuUI");
	
		MainMenuShowAll(false);
		ServerMenuShowAll(true);
    }

	public void MainMenuShowAll(bool active)
    {
		if (active) {
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;
		}
        foreach (GameObject uiElement in GameMenuUI)
        {
            uiElement.SetActive(active);
        }
		foreach (GameObject uiElement in LoadoutUI)
        {
            uiElement.SetActive(active);
        }
	    MainMenuShowing = active;
    }
	
	public void ServerMenuShowAll(bool active)
    {
		if (active) {
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;
		}
        foreach (GameObject uiElement in ServerMenuUI)
        {
            uiElement.SetActive(active);
        }
	    ServerMenuShowing = active;
    }
	
	public void updateLoadout(bool primary)
	{
		/*if (primary){
			primaryWeaponID += 1;
			if(primaryWeaponID == WeaponStats.num_primary_weap)
				primaryWeaponID = 0;
			foreach (GameObject uiElement in LoadoutUI)
			{
				if(uiElement.name.Equals("PrimaryWeaponName"))
					uiElement.GetComponent<Text>().text = WeaponStats.primary_weapon_name[primaryWeaponID];
				if(uiElement.name.Equals("PrimaryWeaponDesc"))
					uiElement.GetComponent<Text>().text = WeaponStats.primary_weapon_desc[primaryWeaponID];
				if(uiElement.name.Equals("PrimaryWeaponStats"))
					uiElement.GetComponent<Text>().text = WeaponStats.primary_weapon_stats[primaryWeaponID];
			}
		}
		else{
			secondaryWeaponID += 1;
			if(secondaryWeaponID == WeaponStats.num_primary_weap)
				secondaryWeaponID = 0;
			foreach (GameObject uiElement in LoadoutUI)
			{
				if(uiElement.name.Equals("SecondaryWeaponName"))
					uiElement.GetComponent<Text>().text = WeaponStats.secondary_weapon_name[secondaryWeaponID];
				if(uiElement.name.Equals("SecondaryWeaponDesc"))
					uiElement.GetComponent<Text>().text = WeaponStats.secondary_weapon_desc[secondaryWeaponID];
				if(uiElement.name.Equals("SecondaryWeaponStats"))
					uiElement.GetComponent<Text>().text = WeaponStats.secondary_weapon_stats[secondaryWeaponID];
			}
		}*/
	}
}