class Terminal():
    def listen(self, commands):
        while True:
            try:
                cmd = input()
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