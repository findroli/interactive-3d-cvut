public class AppState {
    public static AppState shared() {
        if(instance == null) {
            instance = new AppState();
        }
        return instance;
    }
    private static AppState instance;

    public User? currentUser;
    public AppMode mode;
    
    public string modelName;
    public string modelVersionName;
}