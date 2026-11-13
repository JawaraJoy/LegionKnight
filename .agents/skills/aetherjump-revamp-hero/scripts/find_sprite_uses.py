"""Find direct text references to Unity sprite GUIDs. Prints JSON and only reads files."""
import argparse
import base64
import json
import re
import subprocess
from pathlib import Path

GUID = r"[0-9a-f]{32}"
DECLARATION = re.compile(r"^guid:[ \t]*(" + GUID + r")[ \t]*\r?$", re.I | re.M)
ROOTS = ("Assets", "Packages", "ProjectSettings")


def identify(project, argument):
    if re.fullmatch(GUID, argument, re.I):
        return argument.lower()
    path = project / argument
    meta = path if path.suffix.lower() == ".meta" else Path(str(path) + ".meta")
    if not meta.is_file():
        raise ValueError(f"File .meta tidak ditemukan: {meta}")
    data = meta.read_text(encoding="utf-8-sig")
    match = DECLARATION.search(data)
    if not match:
        raise ValueError(f"GUID aset tidak ditemukan: {meta}")
    return match.group(1).lower()


def rg_text(field):
    if "text" in field:
        return field["text"]
    return base64.b64decode(field["bytes"]).decode("utf-8", errors="replace")


def find_uses(project, arguments):
    if not all((project / folder).is_dir() for folder in ("Assets", "ProjectSettings")):
        raise ValueError(f"Root Unity harus memuat Assets dan ProjectSettings: {project}")
    records = {}
    for argument in arguments:
        guid = identify(project, argument)
        records.setdefault(guid, {"assets": [], "uses": []})

    pattern = r"\b(?:" + "|".join(records) + r")\b"
    folders = [folder for folder in ROOTS if (project / folder).is_dir()]
    command = ["rg", "--no-config", "--no-ignore", "--hidden", "--json", "-i", "-e", pattern, "--", *folders]
    try:
        result = subprocess.run(command, cwd=project, capture_output=True)
    except FileNotFoundError as error:
        raise ValueError("Helper ini menggunakan ripgrep (rg) di PATH.") from error
    if result.returncode not in (0, 1):
        raise ValueError(result.stderr.decode("utf-8", errors="replace").strip())

    for raw in result.stdout.splitlines():
        event = json.loads(raw)
        if event["type"] != "match":
            continue
        data = event["data"]
        path = rg_text(data["path"]).replace("\\", "/")
        line = rg_text(data["lines"]).rstrip("\r\n")
        declaration = DECLARATION.fullmatch(line) if path.endswith(".meta") else None
        if declaration:
            guid = declaration.group(1).lower()
            records[guid]["assets"].append(path[:-5])
            continue
        for guid in {rg_text(hit["match"]).lower() for hit in data["submatches"]}:
            records[guid]["uses"].append({"path": path, "line": data["line_number"], "text": line.strip()})

    for record in records.values():
        record["assets"].sort()
        record["uses"].sort(key=lambda hit: (hit["path"], hit["line"]))
    return records


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("sprites", nargs="+", help="Path aset, path .meta, atau GUID Unity.")
    parser.add_argument("--project", type=Path, default=Path.cwd(), help="Root proyek Unity. Default direktori kerja.")
    args = parser.parse_args()
    try:
        records = find_uses(args.project.resolve(), args.sprites)
    except (OSError, ValueError) as error:
        parser.exit(2, f"{error}\n")
    print(json.dumps(records, indent=2))


if __name__ == "__main__":
    main()
