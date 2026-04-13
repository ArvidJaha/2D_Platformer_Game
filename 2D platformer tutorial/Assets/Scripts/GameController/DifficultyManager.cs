using UnityEngine;
using UnityEngine.UI;

public class DifficultyManager : MonoBehaviour
{
    [SerializeField] Slider difficultySlider;

    private float _difficulty;

    public void Start()
    {
        if (!PlayerPrefs.HasKey("difficultyIntensity"))
        {
            PlayerPrefs.SetFloat("difficultyIntensity", 0.5f);
            Load();
        }
        else
        {
            Load();
        }
    }

    public void ChangeDifficulty()
    {
        _difficulty = difficultySlider.value;
        Save();
    }

    public float GetDifficultyIntensity()
    {
        _difficulty = PlayerPrefs.GetFloat("difficultyIntensity");
        return PlayerPrefs.GetFloat("difficultyIntensity");
    }

    private void Load()
    {
        difficultySlider.value = 0.5f;
    }

    private void Save()
    {
        PlayerPrefs.SetFloat("difficultyIntensity", difficultySlider.value);
    }
}
