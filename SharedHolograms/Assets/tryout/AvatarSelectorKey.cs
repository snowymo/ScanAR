using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarSelectorKey : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
    if (Input.GetKeyDown(KeyCode.Alpha1))
    {
      PlayerAvatarStore.Instance.DismissAvatarPicker();

      LocalPlayerManager.Instance.SetUserAvatar(0);
    }
    if (Input.GetKeyDown(KeyCode.Alpha2))
    {
      PlayerAvatarStore.Instance.DismissAvatarPicker();

      LocalPlayerManager.Instance.SetUserAvatar(1);
    }
    if (Input.GetKeyDown(KeyCode.Alpha3))
    {
      PlayerAvatarStore.Instance.DismissAvatarPicker();

      LocalPlayerManager.Instance.SetUserAvatar(2);
    }
    if (Input.GetKeyDown(KeyCode.Alpha4))
    {
      PlayerAvatarStore.Instance.DismissAvatarPicker();

      LocalPlayerManager.Instance.SetUserAvatar(3);
    }
  }
}
