using UnityEngine;

public class GameController : MonoBehaviour {

    // SINGLETON
    public static GameController instance;

    private FourthDimension.Dungeon.DungeonGeneration m_dungeonGeneration;
    private FourthDimension.TurnBased.Actor.Hero m_heroReference;
    private FourthDimension.UI.UserInterfaceManager m_UIManager;

    private void Awake() {
        if(instance == null) {
            instance = this;
            // TODO DontDestroyOnLoad ?
        } else {
            Destroy(gameObject);
        }

        m_dungeonGeneration = FindObjectOfType<FourthDimension.Dungeon.DungeonGeneration>();
        m_heroReference = FindObjectOfType<FourthDimension.TurnBased.Actor.Hero>();
        m_UIManager = FindObjectOfType<FourthDimension.UI.UserInterfaceManager>();
    }

    private void Start() {
        // TODO
        m_heroReference.transform.position = m_dungeonGeneration.StartingPosition;
        m_heroReference.InitializeHero();
    }

    #region Communicating with the UI
    /// <summary>
    /// <para>Sends the UI Manager to render the feedback text</para>
    /// </summary>
    /// <param name="_worldPosition">World Position of where the text will be rendered</param>
    /// <param name="_text">Text condition</param>
    /// <param name="_textColor">Text color</param>
    public void RenderFeedbackText(Vector3 _worldPosition, string _text, Color _textColor) {
        m_UIManager.RenderFeedbackText(_worldPosition, _text, _textColor);
    }
    #endregion
}
