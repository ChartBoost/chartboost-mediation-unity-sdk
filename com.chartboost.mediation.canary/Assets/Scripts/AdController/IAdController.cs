/// <summary>
/// Defines standard methods for all types of ad controllers
/// </summary>
public interface IAdController
{
    /// <summary>
    /// Configure this ad controller.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    void Configure(AdControllerConfiguration configuration);

    /// <summary>
    /// Handler for the back button. Communicate to the parent placement list
    /// controller to remove this ad controller from the object hierarchy.
    /// </summary>
    void OnBackButtonPushed();

    /// <summary>
    /// Handler for the clear log button. This will clear the log completely.
    /// </summary>
    void OnClearLogButtonPushed();

    /// <summary>
    /// Handler for when the load button is pushed.
    /// </summary>
    void OnLoadButtonPushed();

    /// <summary>
    /// Handler for when the show button is pushed.
    /// </summary>
    void OnShowButtonPushed();

    /// <summary>
    /// Handler for when the remove button is pushed.
    /// </summary>
    void OnClearButtonPushed();

    /// <summary>
    /// Handler for when the remove button is pushed.
    /// </summary>
    void OnDestroyButtonPushed();

    /// <summary>
    /// Handler for when the toggle visibility button is pushed.
    /// </summary>
    void OnToggleVisibilityButtonPushed();

    /// <summary>
    /// Handler for when the keywords button is pushed.
    /// </summary>
    void OnKeywordsButtonPushed();
}
