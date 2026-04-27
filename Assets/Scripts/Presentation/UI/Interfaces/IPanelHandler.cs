namespace Presentation.UI.Interfaces
{
    /// <summary>
    ///     面板处理器接口 - 定义面板的生命周期和配置
    /// </summary>
    public interface IPanelHandler
    {
        bool ClosableByEsc { get => true; }

        bool BlocksInput { get => false; }

        void OnPanelShow();

        void OnPanelHide();
    }
}