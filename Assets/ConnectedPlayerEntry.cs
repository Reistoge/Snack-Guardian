using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConnectedPlayerEntry : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private Button inviteButton;

    private string playerId;

    public void Initialize(string id)
    {
        playerId = id;
        playerNameText.text = id;

        inviteButton.onClick.AddListener(() => {
            Debug.Log($"Inviting {playerId}");
            LobbyUIManager.Instance.setOponentId(playerId);
            NetworkManager.Instance.sendPrivateMessage(playerId, "ready-to-play");
        });
    }
}