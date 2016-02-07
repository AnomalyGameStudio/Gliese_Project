using UnityEngine;
using System.Collections;

public interface IUpgradable
{
	bool doubleJump
	{
		get;
		set;
	}

	void EnablePowerUp(string powerUp);
}