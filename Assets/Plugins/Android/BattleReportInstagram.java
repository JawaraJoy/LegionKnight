package com.aetherjump.share;

import android.app.Activity;
import android.content.ClipData;
import android.content.ComponentName;
import android.content.ContentValues;
import android.content.Intent;
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

    public static String open(Activity host, String path, boolean story) {
        PackageManager pm = host.getPackageManager();
        try {
            pm.getApplicationInfo(INSTAGRAM, 0);
        } catch (PackageManager.NameNotFoundException missing) {
            return "Install Instagram, then try again.";
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
                if (editStory.resolveActivity(pm) != null) destination = editStory;
            }
        }
        if (destination == null) {
            for (ResolveInfo result : pm.queryIntentActivities(send, PackageManager.MATCH_DEFAULT_ONLY)) {
                if (result.activityInfo == null || !result.activityInfo.exported) continue;
                String name = result.activityInfo.name.toLowerCase(Locale.ROOT);
                if (story ? name.contains("story") || name.contains("stories") : name.contains("feed")) {
                    destination = new Intent(send).setComponent(new ComponentName(INSTAGRAM, result.activityInfo.name));
                    break;
                }
            }
        }
        String route = story ? "Stories" : "Feed";
        send.setClipData(ClipData.newRawUri("Battle Report", uri));
        send.addFlags(Intent.FLAG_GRANT_READ_URI_PERMISSION);
        if (destination != null) {
            destination.setClipData(send.getClipData());
            destination.addFlags(Intent.FLAG_GRANT_READ_URI_PERMISSION);
            final Intent editor = destination;
            host.runOnUiThread(() -> launchSafely(host, editor));
            return "Finish sharing in Instagram " + route + ".";
        }

        boolean canSend = send.resolveActivity(pm) != null;
        if (Build.VERSION.SDK_INT < 29) {
            if (!canSend) return "Instagram cannot open this report on this device.";
            host.runOnUiThread(() -> launchSafely(host, Intent.createChooser(send, "Choose Instagram " + route)));
            return "Choose " + route + " in Instagram.";
        }
        // Instagram versions expose different destinations. Let the player choose the
        // native picker or deliberately save the image for Instagram's own gallery.
        host.runOnUiThread(() -> {
            android.app.AlertDialog.Builder options = new android.app.AlertDialog.Builder(host)
                .setTitle("Instagram " + route)
                .setMessage("Choose your destination in Instagram, or save this report and select it from your gallery.")
                .setNegativeButton("Cancel", null)
                .setNeutralButton("Save image", (dialog, which) -> {
                    try {
                        saveToPictures(host, file);
                        String guide = "Saved to Pictures/AetherJump. Select it in Instagram " + route + ".";
                        Toast.makeText(host, guide, Toast.LENGTH_LONG).show();
                        Intent launch = pm.getLaunchIntentForPackage(INSTAGRAM);
                        if (launch != null) launchSafely(host, launch);
                    } catch (IOException | RuntimeException unavailable) {
                        Toast.makeText(host, "Could not save the report.", Toast.LENGTH_LONG).show();
                    }
                });
            if (canSend) options.setPositiveButton("Open Instagram",
                (dialog, which) -> launchSafely(host, Intent.createChooser(send, "Choose Instagram " + route)));
            options.show();
        });
        return "Choose " + route + " in Instagram.";
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