from panda3d.core import loadPrcFileData, BamFile, Filename, NodePath
import json, sys, os

def remove_root(filename, root):
    filename = filename.replace('/', os.sep).strip(os.sep)

    if filename.lower().startswith(root):
        filename = filename[len(root)+1:]

    return filename

def list_all_textures(root, path):
    try:
        bam = BamFile()
        bam.open_read(Filename(path))
        obj = NodePath(bam.readNode())
    except:
        return []

    textures = []
    root = root.lower().replace(':', '')

    for texture in obj.find_all_textures():
        for filename in [texture.get_filename(), texture.get_alpha_filename()]:
            if not filename:
                continue

            filename = filename.get_fullpath()

            if filename not in textures:
                textures.append(filename)

    obj.removeNode()
    return [remove_root(texture, root) for texture in textures]

def run_main(root):
    try:
        workload = input()
    except:
        workload = raw_input()

    try:
        workload = json.loads(workload)
    except:
        print('Error: Invalid input given to indexer!')
        sys.exit(1)

    if not isinstance(workload, list):
        print('Error: Input is not a list!')
        sys.exit(1)

    output = {path: list_all_textures(root, path) for path in workload}
    print(json.dumps(output))

if __name__ == '__main__':
    root = os.getcwd()
    run_main(root)