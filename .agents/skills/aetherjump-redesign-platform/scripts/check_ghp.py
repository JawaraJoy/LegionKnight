"""Bandingkan geometri sprite GHP dan kontak rel pada tangkapan Unity."""

import argparse
import json
import math
import sys
from pathlib import Path

import numpy as np
from PIL import Image


def read_mask(path):
    with Image.open(path) as image:
        if image.format != "PNG" or (
            "A" not in image.getbands() and "transparency" not in image.info
        ):
            raise ValueError(f"{path}: gunakan PNG dengan transparansi.")
        alpha = np.asarray(image.convert("RGBA").getchannel("A"))
    mask = alpha >= 128
    if not mask.any() or not (alpha == 0).any():
        raise ValueError(f"{path}: badan atau latar transparan tidak ditemukan.")
    return mask


def bounds(mask):
    rows, cols = np.nonzero(mask)
    return int(cols.min()), int(rows.min()), int(cols.max() + 1), int(rows.max() + 1)


def measure_sprite(path):
    mask = read_mask(path)
    left, top, right, bottom = bounds(mask)
    return {
        "canvas_px": [mask.shape[1], mask.shape[0]],
        "origin_px": [left, top],
        "body_px": [right - left, bottom - top],
    }


def measure_joint(lower, upper, pitch, bevel):
    if lower.shape != upper.shape:
        raise ValueError("Gunakan kamera dan ukuran kanvas yang sama untuk semua baris.")
    lx, ly, lr, _ = bounds(lower)
    ux, uy, ur, _ = bounds(upper)
    left = max(lx, ux) + bevel
    right = min(lr, ur) - bevel
    if right <= left:
        raise ValueError("Tidak ada irisan rel untuk diperiksa dengan margin bevel ini.")

    low = lower[:, left:right]
    high = upper[:, left:right]
    occupied = low.any(axis=0) & high.any(axis=0)
    top_of_lower = low.argmax(axis=0)
    bottom_of_upper = high.shape[0] - high[::-1].argmax(axis=0)
    gaps = top_of_lower - bottom_of_upper  # A positive value counts empty pixels.

    # Check the seam itself too: an interior notch can escape outer-edge measurements.
    seam = slice(max(0, ly - 1), min(ly + 2, low.shape[0]))
    sealed = (low[seam] | high[seam]).all(axis=0)
    open_columns = ~occupied | (gaps > 0) | ~sealed
    actual_pitch = ly - uy
    pitch_ok = actual_pitch > 0 and math.floor(pitch) <= actual_pitch <= math.ceil(pitch)
    return {
        "pitch_px": actual_pitch,
        "columns_checked": right - left,
        "open_columns_px": int(open_columns.sum()),
        "max_edge_gap_px": max(0, int(gaps[occupied].max())) if occupied.any() else None,
        "passed": pitch_ok and not bool(open_columns.any()),
    }


def measure_stack(paths, pitch, bevel):
    if len(paths) < 2 or not math.isfinite(pitch) or pitch <= 0 or bevel < 0:
        raise ValueError("Butuh ≥2 baris, pitch positif dan bevel ≥0.")
    joints = []
    lower = read_mask(paths[0])
    for index, path in enumerate(paths[1:]):
        upper = read_mask(path)
        joint = measure_joint(lower, upper, pitch, bevel)
        joint["rows"] = [paths[index].name, path.name]
        joints.append(joint)
        lower = upper
    return {"passed": all(joint["passed"] for joint in joints), "joints": joints}


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    modes = parser.add_subparsers(dest="mode", required=True)
    sprite = modes.add_parser("sprite", help="Bandingkan kanvas, posisi dan ukuran badan.")
    sprite.add_argument("reference", type=Path)
    sprite.add_argument("candidate", type=Path)
    stack = modes.add_parser("stack", help="Ukur jarak dan kontak antarbaris.")
    stack.add_argument("--pitch-px", type=float, required=True, help="Jarak baris dalam piksel kamera.")
    stack.add_argument("--bevel-px", type=int, default=0, help="Lebar ujung di luar rel datar per sisi.")
    stack.add_argument("rows", nargs="+", type=Path, help="Mask baris dari bawah ke atas.")
    args = parser.parse_args()
    try:
        if args.mode == "sprite":
            reference = measure_sprite(args.reference)
            candidate = measure_sprite(args.candidate)
            result = {"passed": reference == candidate, "reference": reference, "candidate": candidate}
        else:
            result = measure_stack(args.rows, args.pitch_px, args.bevel_px)
    except (OSError, ValueError) as error:
        parser.exit(2, f"{parser.prog}: {error}\n")
    print(json.dumps(result, indent=2))
    return 0 if result["passed"] else 1


if __name__ == "__main__":
    sys.exit(main())
