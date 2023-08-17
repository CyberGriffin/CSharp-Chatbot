# """
# Entry point for the Python API.
# """

from flask import Flask, jsonify, request
from Utils.indexer import index
from Utils.search_index import search_documents

app = Flask(__name__)

@app.route('/indexer', methods=['POST'])
def indexer():
    index()
    return jsonify({"message": "Documents indexed."})

@app.route('/search_index', methods=['POST'])
def search_index():
    query = request.json['query']
    response = search_documents(query)
    return jsonify({"message": response})

if __name__ == '__main__':
    app.run(debug=True)
