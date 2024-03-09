using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SheepManager : MonoBehaviour
{
    public GameObject end;

    public static SheepManager instance;
    public List<Sheep> sheeps = new List<Sheep>();

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

        if (sheeps.Count == 0)
        {
            end.SetActive(true);

            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }

    public static void KillSheep(Sheep sheep)
    {
        instance.sheeps.Remove(sheep);
        Destroy(sheep.gameObject);
    }

}
