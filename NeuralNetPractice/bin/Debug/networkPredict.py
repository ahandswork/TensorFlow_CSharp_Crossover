from __future__ import absolute_import, division, print_function, unicode_literals

import pathlib
import os

#import matplotlib.pyplot as plt
import pandas as pd
import seaborn as sns

import tensorflow as tf
import numpy
from tensorflow import keras
from tensorflow.keras import layers

MODEL_SAVE_LOCATION = "Models\\main_model"

latestSet = pd.read_csv("PostProcessData/latest.csv")

currentModel = keras.models.load_model(MODEL_SAVE_LOCATION)

predictions = currentModel.predict(latestSet)
print("Predicted value for TXN.close: ",predictions[0][0])
