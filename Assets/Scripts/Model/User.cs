public struct User {
    public enum UserType {
        presenter, admin, editor
    }
    public enum Approval {
        yes, pending, declined
    }
    
    public string id { get; }
    public string email { get; }
    public string password { get; }
    public UserType userType { get; }
    public string name { get; }
    public Approval approved { get; }

    public User(string email, string password, UserType userType) {
        this.email = email;
        this.password = password;
        this.userType = userType;
        this.id = "";
        this.name = "";
        this.approved = Approval.yes;
    }

    public User(UserDB userDb, string id, string email, string password) {
        name = userDb.name;
        approved = StringToApproval(userDb.approved);
        userType = StringToType(userDb.type);
        this.id = id;
        this.email = email;
        this.password = password;
    }

    private static UserType StringToType(string type) {
        switch (type) {
            case "presenter":
                return UserType.presenter;
            case "admin":
                return UserType.admin;
            case "editor":
                return UserType.editor;
            default:
                return UserType.presenter;
        }
    }

    private static Approval StringToApproval(string approval) {
        switch (approval) {
            case "yes":
                return Approval.yes;
            case "pending":
                return Approval.pending;
            case "declined":
                return Approval.declined;
            default:
                return Approval.yes;
        }
    }
}