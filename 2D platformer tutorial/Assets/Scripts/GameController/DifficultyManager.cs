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
            PlayerPrefs.SetFloat("difficultyIntensity", 1);
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
        return _difficulty;
    }

    private void Load()
    {
        difficultySlider.value = PlayerPrefs.GetFloat("difficultyIntensity");
    }

    private void Save()
    {
        PlayerPrefs.SetFloat("difficultyIntensity", difficultySlider.value);
    }
}
