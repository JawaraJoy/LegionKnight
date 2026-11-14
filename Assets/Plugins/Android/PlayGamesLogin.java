package com.jawarajoy.aetherjump.auth;

import android.app.Activity;
import androidx.annotation.Keep;
import com.google.android.gms.games.AuthenticationResult;
import com.google.android.gms.games.GamesSignInClient;
import com.google.android.gms.games.PlayGames;
import com.google.android.gms.games.PlayGamesSdk;
import com.google.android.gms.tasks.Task;

@Keep
public final class PlayGamesLogin {
    @Keep
    public interface Callback { void complete(String code); }

    private PlayGamesLogin() { }

    public static void request(Activity activity, boolean interactive, Callback callback) {
        if (activity == null) { callback.complete(null); return; }
        activity.runOnUiThread(() -> {
            try {
                PlayGamesSdk.initialize(activity.getApplicationContext());
                GamesSignInClient client = PlayGames.getGamesSignInClient(activity);
                client.isAuthenticated().addOnCompleteListener(check -> {
                    if (authenticated(check)) requestCode(activity, client, callback);
                    else if (interactive) {
                        try {
                            client.signIn().addOnCompleteListener(retry -> {
                                if (authenticated(retry)) requestCode(activity, client, callback);
                                else callback.complete(null);
                            });
                        } catch (RuntimeException failure) { callback.complete(null); }
                    }
                    else callback.complete(null);
                });
            } catch (RuntimeException failure) { callback.complete(null); }
        });
    }

    private static boolean authenticated(Task<AuthenticationResult> result) {
        return result.isSuccessful() && result.getResult() != null && result.getResult().isAuthenticated();
    }

    private static void requestCode(Activity activity, GamesSignInClient client, Callback callback) {
        try {
            int id = activity.getResources().getIdentifier("aether_play_games_web_client", "string", activity.getPackageName());
            client.requestServerSideAccess(activity.getString(id), false).addOnCompleteListener(result ->
                callback.complete(result.isSuccessful() ? result.getResult() : null));
        } catch (RuntimeException failure) { callback.complete(null); }
    }
}
