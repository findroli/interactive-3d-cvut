using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;
using UnityEngine.Events;

public class DBManager {
    public static DBManager shared() {
        if(instance == null) {
            instance = new DBManager();
        }
        return instance;
    }
    private static DBManager instance;

    public void LoginFirebase(string email, string password, UnityAction<bool, string> callback) {
        FirebaseAuth.DefaultInstance.SignInWithEmailAndPasswordAsync(email, password)
            .ContinueWithOnMainThread(task => {
            if (task.IsCanceled || task.IsFaulted) {
                callback(false, task.Exception?.Message ?? "Sign in failed (unknown reason)");
                return;
            }
            callback(true, task.Result.UserId);
        });
    }

    public void GetUser(string id, string email, string password, UnityAction<User> callback) {
        FirebaseDatabase.DefaultInstance.RootReference.Child("users").Child(id).GetValueAsync()
            .ContinueWithOnMainThread(t => {
            if (t.IsCompleted) {
                DataSnapshot snapshot = t.Result;
                if (snapshot == null) return;
                var userDb = JsonUtility.FromJson<UserDB>(snapshot.GetRawJsonValue());
                var user = new User(userDb, id, email, password);
                callback(user);
            }
        });
    }

    public void RegisterFirebase(string username, string email, string password, UnityAction<bool> callback) {
        FirebaseAuth.DefaultInstance.CreateUserWithEmailAndPasswordAsync(email, password)
            .ContinueWithOnMainThread(task => {
            if (task.IsCanceled || task.IsFaulted) {
                callback(false);
                return;
            }
            AddUser(task.Result.UserId, username, success => callback(success));
            });
    }

    public void AddUser(string id, string username, UnityAction<bool> callback) {
        var userDb = new UserDB();
        userDb.name = username;
        userDb.approved = "no";
        userDb.type = "editor";
        string json = JsonUtility.ToJson(userDb);
        FirebaseDatabase.DefaultInstance.RootReference.Child("users").Child(id).SetRawJsonValueAsync(json)
            .ContinueWithOnMainThread(t => callback(t.IsCompleted));
    }

    private DBManager() {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available) {
                Debug.Log("Firebase ready to use!");
            } else {
                Debug.Log("Firebase init failed!");
            }
        });
    }
}
