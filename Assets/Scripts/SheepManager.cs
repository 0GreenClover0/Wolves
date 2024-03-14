using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SheepManager : MonoBehaviour
{
    public GameObject end;

    public static SheepManager instance;
    [NonSerialized] public List<Sheep> sheeps = new List<Sheep>();
    public Player player;

    public TextMeshProUGUI sheepCountText;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        for (int i = sheeps.Count - 1; i >= 0; --i)
        {
            if (sheeps[i] == null)
                sheeps.RemoveAt(i);
        }

        if (sheeps.Count == 0 && LevelManager.instance.levelIsRunning)
        {
            end.SetActive(true);

            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
        sheepCountText.text = "Sheep: " + sheeps.Count;
    }

    public static void KillSheep(Sheep sheep)
    {
        instance.sheeps.Remove(sheep);
        Destroy(sheep.gameObject);
    }


    public static void RemoveSheeps()
    {
        for (int i = instance.sheeps.Count - 1; i >= 0; --i)
        {
            Destroy(instance.sheeps[i].gameObject);
            instance.sheeps.RemoveAt(i);
        }
    }

    public Sheep findSheepToSaddle(Vector3 position)
    {
        for(int i = 0; i < instance.sheeps.Count; ++i)
        {
            if (instance.sheeps[i].doShit == true)
            {
                if(Vector3.Distance(position, instance.sheeps[i].transform.position) < 1.0f)
                {
                    return instance.sheeps[i];
                }
            }
        }
        return null;
    }

}
