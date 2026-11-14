package com.aetherjump.share;

import android.app.Activity;
import android.content.ClipData;
import android.content.ComponentName;
import android.content.ContentValues;
import android.content.Intent;
import android.content.SharedPreferences;
import android.content.pm.ApplicationInfo;
import android.content.pm.PackageManager;
import android.content.pm.ResolveInfo;
import android.net.Uri;
import android.os.Build;
import android.os.Bundle;
import android.os.Environment;
import android.provider.MediaStore;
import android.widget.Toast;
import androidx.annotation.Keep;
import androidx.core.content.FileProvider;
import java.io.File;
import java.io.FileInputStream;
import java.io.IOException;
import java.io.OutputStream;
import java.util.Locale;

@Keep
public final class BattleReportInstagram {
    private static final String INSTAGRAM = "com.instagram.android";
    private static final String PNG = "image/png";
    private BattleReportInstagram() {}

    public static String newReportPath(Activity host) throws IOException {
        File dir = new File(host.getCacheDir(), "battle-reports");
        if (!dir.isDirectory() && !dir.mkdirs()) throw new IOException("Cannot create report cache");
        File[] previous = dir.listFiles();
        long cutoff = System.currentTimeMillis() - 2L * 24 * 60 * 60 * 1000;
        if (previous != null) for (File f : previous) {
            if (f.isFile() && f.lastModified() < cutoff) f.delete();
        }
        return File.createTempFile("AetherJump-", ".png", dir).getAbsolutePath();
    }

    @Keep public interface Callback { @Keep void complete(String outcome); }

    public static void open(Activity host, String path, boolean story, String rewardAccount, Callback callback) {
        host.runOnUiThread(() -> {
            try { openOnUiThread(host, path, story, rewardAccount, callback); }
            catch (RuntimeException unavailable) { callback.complete("failed"); }
        });
    }

    // Persist a successful image handoff even if Android reclaims Unity while Instagram is open.
    public static long pendingLaunch(Activity host, String account) {
        return host.getSharedPreferences("aj.battle.share", 0).getLong(account, 0L);
    }

    public static synchronized long pendingAge(Activity host, String account, long stamp) {
        SharedPreferences prefs = host.getSharedPreferences("aj.battle.share", 0);
        if (prefs.getLong(account, 0L) != stamp) return -1;
        long uptime = prefs.getLong(account + ".elapsed", -1L);
        long now = android.os.SystemClock.elapsedRealtime();
        if (uptime >= 0 && now >= uptime && Math.abs((System.currentTimeMillis() - stamp) - (now - uptime)) < 60000)
            return now - uptime;
        // After reboot, trust the calendar only to expire old handoffs; never claim a future launch.
        return Math.max(0L, System.currentTimeMillis() - stamp);
    }

    public static synchronized void acknowledgeLaunch(Activity host, String account, long stamp) {
        if (pendingLaunch(host, account) == stamp)
            host.getSharedPreferences("aj.battle.share", 0).edit().remove(account).remove(account + ".elapsed").commit();
    }

    private static synchronized void launched(Activity host, String account, Callback callback) {
        if (account != null && !account.isEmpty())
            host.getSharedPreferences("aj.battle.share", 0).edit().putLong(account, System.currentTimeMillis())
                .putLong(account + ".elapsed", android.os.SystemClock.elapsedRealtime()).commit();
        callback.complete("opened");
    }

    private static void openOnUiThread(Activity host, String path, boolean story, String rewardAccount, Callback callback) {
        PackageManager pm = host.getPackageManager();
        try {
            pm.getApplicationInfo(INSTAGRAM, 0);
        } catch (PackageManager.NameNotFoundException missing) {
            callback.complete("missing");
            return;
        }
        File file = new File(path);
        Uri uri = FileProvider.getUriForFile(host, host.getPackageName() + ".battlereports", file);
        Intent send = new Intent(Intent.ACTION_SEND).setType(PNG).setPackage(INSTAGRAM);
        send.putExtra(Intent.EXTRA_STREAM, uri);
        Intent destination = null;
        if (story) {
            String id = metaAppId(host);
            if (!id.isEmpty()) {
                Intent editStory = new Intent("com.instagram.share.ADD_TO_STORY").setPackage(INSTAGRAM);
                editStory.setType(PNG);
                editStory.putExtra("interactive_asset_uri", uri);
                editStory.putExtra("source_application", id);
                editStory.putExtra("top_background_color", "#282641");
                editStory.putExtra("bottom_background_color", "#282641");
                ResolveInfo storyTarget = pm.resolveActivity(editStory, PackageManager.MATCH_DEFAULT_ONLY);
                if (isInstagram(storyTarget))
                    destination = editStory.setComponent(new ComponentName(INSTAGRAM, storyTarget.activityInfo.name));
            }
        }
        if (destination == null) {
            for (ResolveInfo result : pm.queryIntentActivities(send, PackageManager.MATCH_DEFAULT_ONLY)) {
                if (!isInstagram(result)) continue;
                String name = result.activityInfo.name.toLowerCase(Locale.ROOT);
                if (story ? name.contains("story") || name.contains("stories") : name.contains("feed")) {
                    destination = new Intent(send).setComponent(new ComponentName(INSTAGRAM, result.activityInfo.name));
                    break;
                }
            }
        }
        String route = story ? "Stories" : "Feed";
        send.setClipData(ClipData.newRawUri("AetherJump Report", uri));
        send.addFlags(Intent.FLAG_GRANT_READ_URI_PERMISSION);
        if (destination != null) {
            destination.setClipData(send.getClipData());
            destination.addFlags(Intent.FLAG_GRANT_READ_URI_PERMISSION);
            host.startActivity(destination);
            launched(host, rewardAccount, callback);
            return;
        }

        java.util.List<ResolveInfo> targets = new java.util.ArrayList<>();
        for (ResolveInfo target : pm.queryIntentActivities(send, PackageManager.MATCH_DEFAULT_ONLY))
            if (isInstagram(target)) targets.add(target);
        if (targets.isEmpty() && Build.VERSION.SDK_INT < 29) { callback.complete("failed"); return; }
        android.app.AlertDialog.Builder options = new android.app.AlertDialog.Builder(host)
            .setTitle("Instagram " + route)
            .setMessage("Choose the destination in Instagram, or save the report to your gallery.")
            .setNegativeButton("Cancel", (dialog, which) -> callback.complete("cancelled"))
            .setOnCancelListener(dialog -> callback.complete("cancelled"));
        if (!targets.isEmpty()) options.setPositiveButton("Open Instagram", (dialog, which) -> {
            if (targets.size() == 1) {
                launchImage(host, send, targets.get(0), rewardAccount, callback);
                return;
            }
            String[] labels = new String[targets.size()];
            for (int i = 0; i < targets.size(); i++) labels[i] = targets.get(i).loadLabel(pm).toString();
            new android.app.AlertDialog.Builder(host).setTitle("Choose Instagram destination")
                .setItems(labels, (picker, index) -> launchImage(host, send, targets.get(index), rewardAccount, callback))
                .setNegativeButton("Cancel", (picker, index) -> callback.complete("cancelled"))
                .setOnCancelListener(picker -> callback.complete("cancelled")).show();
        });
        if (Build.VERSION.SDK_INT >= 29) options.setNeutralButton("Save image", (dialog, which) -> {
            try {
                saveToPictures(host, file);
                callback.complete("saved");
                Toast.makeText(host, "Saved to Pictures/AetherJump.", Toast.LENGTH_LONG).show();
                Intent launch = pm.getLaunchIntentForPackage(INSTAGRAM);
                if (launch != null) launchSafely(host, launch);
            } catch (IOException | RuntimeException error) { callback.complete("failed"); }
        });
        options.show();
    }

    private static boolean isInstagram(ResolveInfo info) {
        return info != null && info.activityInfo != null && info.activityInfo.exported
            && INSTAGRAM.equals(info.activityInfo.packageName);
    }

    private static void launchImage(Activity host, Intent send, ResolveInfo target, String account, Callback callback) {
        try {
            send.setComponent(new ComponentName(INSTAGRAM, target.activityInfo.name));
            host.startActivity(send);
            launched(host, account, callback);
        } catch (RuntimeException unavailable) { callback.complete("failed"); }
    }

    private static String metaAppId(Activity host) {
        try {
            ApplicationInfo data = host.getPackageManager().getApplicationInfo(host.getPackageName(), PackageManager.GET_META_DATA);
            Bundle values = data.metaData;
            Object value = values == null ? null : values.get("com.facebook.sdk.ApplicationId");
            if (value instanceof Integer) value = host.getString((Integer) value);
            String id = value == null ? "" : value.toString().replaceFirst("^fb", "");
            return id.matches("[1-9][0-9]{4,}") ? id : "";
        } catch (Exception ignored) { return ""; }
    }

    private static void launchSafely(Activity host, Intent intent) {
        try { host.startActivity(intent); }
        catch (RuntimeException unavailable) {
            Toast.makeText(host, "Could not open Instagram. Please try again.", Toast.LENGTH_LONG).show();
        }
    }

    private static void saveToPictures(Activity host, File file) throws IOException {
        ContentValues values = new ContentValues();
        values.put(MediaStore.Images.Media.DISPLAY_NAME, file.getName());
        values.put(MediaStore.Images.Media.MIME_TYPE, PNG);
        values.put(MediaStore.Images.Media.RELATIVE_PATH, Environment.DIRECTORY_PICTURES + "/AetherJump");
        values.put(MediaStore.Images.Media.IS_PENDING, 1);
        Uri saved = host.getContentResolver().insert(MediaStore.Images.Media.EXTERNAL_CONTENT_URI, values);
        if (saved == null) throw new IOException("Cannot create image");
        try {
            try (OutputStream out = host.getContentResolver().openOutputStream(saved);
                 FileInputStream in = new FileInputStream(file)) {
                if (out == null) throw new IOException("Cannot open image");
                byte[] buf = new byte[16384];
                for (int count; (count = in.read(buf)) != -1;) out.write(buf, 0, count);
            }
            values.clear();
            values.put(MediaStore.Images.Media.IS_PENDING, 0);
            if (host.getContentResolver().update(saved, values, null, null) < 1) throw new IOException("Cannot publish image");
        } catch (IOException | RuntimeException error) {
            host.getContentResolver().delete(saved, null, null);
            throw error;
        }
    }
}
