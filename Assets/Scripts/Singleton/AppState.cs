public class AppState {
    public static AppState shared() {
        if(instance == null) {
            instance = new AppState();
        }
        return instance;
    }
    private static AppState instance;

    public delegate void OnModeChange();
    public event OnModeChange onModeChange;

    public User? CurrentUser;
    
    private AppMode mode;
    public AppMode Mode {
        get => mode;
        set => SetMode(value);
    }
    
    public string ModelName;
    public string ModelVersionName;

    public void SetMode(AppMode newMode) {
        mode = newMode;
        onModeChange?.Invoke();
    }
}