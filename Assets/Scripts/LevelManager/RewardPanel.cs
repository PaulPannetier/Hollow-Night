using UnityEngine;

public class RewardPanel : MonoBehaviour
{
    [SerializeField] private Transform killerPanel;
    [SerializeField] private Transform poulpePanel;
    [SerializeField] private Transform survivorPanel;
    [SerializeField] private Transform PoissonGlobePanel;

    private enum PanelState
    {
        None,
        Killer,
        Poulpe,
        Survivor,
        PoissonGlobe
    }

    private PanelState currentState = PanelState.None;

    void Start()
    {
        SwitchPanel(PanelState.None);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CyclePanels();
        }
    }

    private void CyclePanels()
    {
        switch (currentState)
        {
            case PanelState.None:
                SwitchPanel(PanelState.Killer);
                break;
            case PanelState.Killer:
                SwitchPanel(PanelState.Poulpe);
                break;
            case PanelState.Poulpe:
                SwitchPanel(PanelState.Survivor);
                break;
            case PanelState.Survivor:
                SwitchPanel(PanelState.PoissonGlobe);
                break;
            case PanelState.PoissonGlobe:
                SwitchPanel(PanelState.None);
                break;
        }
    }
    private void SwitchPanel(PanelState newState)
    {
        if (currentState == newState)
            return;

        DiscardAllPanels();

        currentState = newState;
        switch (newState)
        {
            case PanelState.Killer:
                killerPanel.gameObject.SetActive(true);
                break;
            case PanelState.Poulpe:
                poulpePanel.gameObject.SetActive(true);
                break;
            case PanelState.Survivor:
                survivorPanel.gameObject.SetActive(true);
                break;
            case PanelState.PoissonGlobe:
                PoissonGlobePanel.gameObject.SetActive(true);
                break;
            case PanelState.None:
                // Aucun panneau actif
                break;
        }
    }

    private void DiscardAllPanels()
    {
        killerPanel.gameObject.SetActive(false);
        poulpePanel.gameObject.SetActive(false);
        survivorPanel.gameObject.SetActive(false);
    }
}
