from flask import Flask, request

app = Flask(__name__)

@app.route("/checkin", methods = ['GET'])
def checkin():
    return "whoami"

@app.route("/response", methods = ['POST'])
def response():
    print(request.data)
    return "ok"


if __name__ == '__main__':
    app.run()