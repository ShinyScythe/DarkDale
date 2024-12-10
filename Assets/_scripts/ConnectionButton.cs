using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class ConnectButton : MonoBehaviour
{
    public Button connectButton;
    public TMP_InputField usernameField;

    void Start()
    {
        // Get the Button component if not already assigned in the inspector
        if (connectButton == null)
        {
            connectButton = GetComponent<Button>();
        }

        // Ensure the button exists and add the listener
        if (connectButton != null)
        {
            connectButton.onClick.AddListener(AuthenticateUser);
        }
        else
        {
            Debug.LogError("ConnectButton is not assigned or missing!");
        }
    }

    void AuthenticateUser()
    {
        if (usernameField != null)
        {
            // Check if input field has a value
            string username = usernameField.text;

            if (!string.IsNullOrWhiteSpace(username))
            {
                // Save the username in GameManager
                GameManager.Instance.PlayerUsername = username;
                

                // Authenticate user
                Debug.Log($"Authenticating user: {username}");

                // Change scene
                SceneManager.LoadScene("_scenes/Zone1");
            }
            else
            {
                Debug.LogWarning("username is empty. Please enter a username.");
            }
        }
        else
        {
            Debug.LogError("username Input Field is missing!");
        }
    }
}

