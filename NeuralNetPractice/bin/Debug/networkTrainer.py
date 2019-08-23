from __future__ import absolute_import, division, print_function, unicode_literals

import pathlib

#import matplotlib.pyplot as plt
import pandas as pd
import seaborn as sns

import tensorflow as tf
from tensorflow import keras
from tensorflow.keras import layers

import sys
EPOCHS = int(sys.argv[1])

MODEL_SAVE_LOCATION = "Models\main_model"
CHECKPOINT_SAVE_LOCATION = "Models\checkpoint_model"

print("Importing Data: Started")
#importing data
trainingSet = pd.read_csv("PostProcessData/trainingSet.csv")
trainingLabelSet = pd.read_csv("PostProcessData/trainingLabelSet.csv")

#splitting data into training data and test data
#trainingSet = dataset.sample(frac=0.8,random_state=0)
#test_dataset = dataset.drop(trainingSet.index)

print("Importing Data: Complete")
print("Building Model: Started")

#normalizing data
#def norm(x):
#    return (x - train_stats['mean'])/train_stats['std']
#trainingSet = norm(trainingSet)
#testingSet = norm(test_dataset)

#THE MODEL

#Build the model
def build_model():
  model = keras.Sequential([
    layers.Dense(512, activation=tf.nn.relu, input_shape=[len(trainingSet.keys())]),
    layers.Dense(1024, activation=tf.nn.relu),
    layers.Dense(400, activation=tf.nn.relu),
    layers.Dense(200, activation=tf.nn.relu),
    layers.Dense(64, activation=tf.nn.relu),
    layers.Dense(1)
  ])

  optimizer = tf.keras.optimizers.RMSprop(0.0001,0.9,0.001)

  model.compile(loss='mean_squared_error',
                optimizer=optimizer,
                metrics=['mean_absolute_error', 'mean_squared_error'])
  return model

model = build_model()

#inspecting the model
model.summary()

#training the model
# Display training progress by printing a single dot for each completed epoch
class PrintDot(keras.callbacks.Callback):
  def on_epoch_end(self, epoch, logs):
    print(epoch/EPOCHS * 100,"%")
    if epoch % 10 == 0: 
        model.save(CHECKPOINT_SAVE_LOCATION)



history = model.fit(
  trainingSet, trainingLabelSet,
  epochs=EPOCHS, validation_split = 0.2, verbose=0,
  callbacks=[PrintDot()])

print("training complete")

hist = pd.DataFrame(history.history)
hist['epoch'] = history.epoch
hist.tail()

#model = build_model()

# The patience parameter is the amount of epochs to check for improvement
#early_stop = keras.callbacks.EarlyStopping(monitor='val_loss', patience=10)
#
#history = model.fit(trainingSet, trainingLabelSet, epochs=EPOCHS,
#                    validation_split = 0.2, verbose=0, callbacks=[early_stop, PrintDot()])
#
#plot_history(history)
model.save(MODEL_SAVE_LOCATION)