using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Sticker : MonoBehaviour {
    public StickClick stickEvent;
    [SerializeField] StickerManager manager;
	// Use this for initialization
	void Start () {
        stickEvent = new StickClick();
        stickEvent.AddListener(manager.SetSprite);
	}

    public void Invoke()
    {
        stickEvent.Invoke(gameObject.GetComponent<Image>().sprite);
    }
}

public class StickClick : UnityEvent<Sprite>{}
