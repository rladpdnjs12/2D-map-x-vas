using Niantic.Lightship.SharedAR.Colocalization;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class MySharedSpaceController : MonoBehaviour
{
    [SerializeField]
    private SharedSpaceManager _sharedSpaceManager;

    [SerializeField]
    private Button _joinAsHostButton;

    [SerializeField]
    private Button _joinAsClientButton;

    private bool _started = false; // ���� ���� ������ ���۵Ǿ����� ���θ� ��Ÿ���� ������ �߰��մϴ�.

    private void Start()
    {
        _sharedSpaceManager.sharedSpaceManagerStateChanged += OnColocalizationTrackingStateChanged;
        _joinAsHostButton.onClick.AddListener(OnJoinAsHostClicked);
        _joinAsClientButton.onClick.AddListener(OnJoinAsClientClicked);
        // �ʱ� UI ���� ����
        _joinAsHostButton.gameObject.SetActive(false);
        _joinAsClientButton.gameObject.SetActive(false);
    }

    private void OnJoinAsHostClicked()
    {
        NetworkManager.Singleton.StartHost();
        HideButtons();
    }

    private void OnJoinAsClientClicked()
    {
        NetworkManager.Singleton.StartClient();
        HideButtons();
    }

    private void HideButtons()
    {
        _joinAsHostButton.gameObject.SetActive(false);
        _joinAsClientButton.gameObject.SetActive(false);
    }

    private void OnColocalizationTrackingStateChanged(SharedSpaceManager.SharedSpaceManagerStateChangeEventArgs args)
    {
        if (args.Tracking)
        {
            _joinAsHostButton.gameObject.SetActive(true);
            _joinAsClientButton.gameObject.SetActive(true);
        }
        else
        {
            // ���� ���� ������ �Ұ����� �� ��ư�� ��Ȱ��ȭ�� ���� �ֽ��ϴ�.
            _joinAsHostButton.gameObject.SetActive(false);
            _joinAsClientButton.gameObject.SetActive(false);
        }
    }
}
