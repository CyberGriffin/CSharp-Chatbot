"""
Script responsible for indexing documents with FAISS.
"""

import os
from langchain.vectorstores import FAISS
from langchain.document_loaders import PyPDFLoader
from langchain.document_loaders import Docx2txtLoader
from langchain.embeddings.openai import OpenAIEmbeddings

embedding_model = os.getenv("OPENAI_EMBEDDING_DEPLOYMENT_NAME")
embedding = OpenAIEmbeddings(model=embedding_model, chunk_size=1, disallowed_special=())

base_path = os.path.dirname(os.path.normpath(os.getcwd()))
document_path = os.path.join(base_path, "data\\docs")

def index():
    """
    Index documents with FAISS.
    """
    # Get the documents to index and initialize array to store loaded pages
    documents = os.listdir(document_path)

    pages = []

    # Load the documents and split them into pages
    for file in documents:
        # Get the file path
        file_path = os.path.join(document_path, file)

        # Prepare the loader
        if file.endswith(".pdf"):
            loader = PyPDFLoader(file_path)
        elif file.endswith(".docx"):
            loader = Docx2txtLoader(file_path)
        else:
            continue

        # Load the pages
        pages.extend(loader.load_and_split())

    # Index the pages into a vectorstore using FAISS
    vectorstore = FAISS.from_documents(pages, embedding)
    vectorstore.save_local(os.path.join(base_path, "data\\faiss"))
