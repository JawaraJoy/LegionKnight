package com.jawarajoy.aetherjump.review;

import android.app.Activity;
import android.content.Context;
import android.content.SharedPreferences;
import android.util.Log;
import androidx.annotation.Keep;
import com.google.android.gms.tasks.Task;
import com.google.android.play.core.review.ReviewInfo;
import com.google.android.play.core.review.ReviewManager;
import com.google.android.play.core.review.ReviewManagerFactory;
import java.util.concurrent.TimeUnit;

@Keep
public final class PlayReview {
    private static final String PREFS = "aetherjump.play.review";
    private static final String LAST_FLOW = "lastFlowCompletedAt";
    private static final long INTERVAL = TimeUnit.DAYS.toMillis(90);
    // Accessed on the Android main thread, including Google's completion listeners.
    private static boolean inFlight;

    private PlayReview() { }

    public static void request(Activity activity) {
        if (activity != null) activity.runOnUiThread(() -> requestOnUiThread(activity));
    }

    private static boolean canPresent(Activity activity) {
        return !activity.isFinishing() && !activity.isDestroyed() && activity.hasWindowFocus();
    }

    private static void requestOnUiThread(Activity activity) {
        if (inFlight || !canPresent(activity)) return;
        SharedPreferences prefs = activity.getSharedPreferences(PREFS, Context.MODE_PRIVATE);
        long last = prefs.getLong(LAST_FLOW, 0);
        long now = System.currentTimeMillis();
        if (last > 0 && now >= last && now - last < INTERVAL) return;
        inFlight = true;
        try {
            ReviewManager manager = ReviewManagerFactory.create(activity);
            manager.requestReviewFlow().addOnCompleteListener(info -> launch(activity, manager, info, prefs));
        } catch (RuntimeException error) {
            unavailable(error);
        }
    }

    private static void launch(Activity activity, ReviewManager manager, Task<ReviewInfo> info, SharedPreferences prefs) {
        if (!info.isSuccessful() || !canPresent(activity)) {
            unavailable(info.getException());
            return;
        }
        try {
            manager.launchReviewFlow(activity, info.getResult()).addOnCompleteListener(flow -> {
                inFlight = false;
                if (flow.isSuccessful()) {
                    // Play doesn't disclose whether a dialog appeared or a review was submitted.
                    prefs.edit().putLong(LAST_FLOW, System.currentTimeMillis()).apply();
                } else {
                    unavailable(flow.getException());
                }
            });
        } catch (RuntimeException error) {
            unavailable(error);
        }
    }

    private static void unavailable(Exception error) {
        inFlight = false;
        if (error != null) Log.w("AetherJumpReview", "Google Play review flow unavailable", error);
    }
}
