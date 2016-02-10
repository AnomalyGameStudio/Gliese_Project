using UnityEngine;
using System.Collections;

public interface IEntity
{
	IDamageable<float> stats
	{
		get;
	}
}
