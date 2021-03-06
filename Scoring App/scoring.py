# SPDX-License-Identifier: GPL-3.0-or-later

from tkinter import *


class ScoreBoard(Frame):
    DEFAULT_GAME_LENGTH = 600  # seconds

    def __init__(self, parent=None):
        self.running = False
        self.timeLeft = ScoreBoard.DEFAULT_GAME_LENGTH
        self.scoreRed = 0
        self.scoreBlue = 0
        self.timer = None
        Frame.__init__(self, parent)
        self.nameRedLabel = Label(parent, text='Red', relief='sunken',
                                  bg='red', fg='white',
                                  font=('Helvetica', 50, 'bold'))
        self.timeLabel = Label(parent, text='00:00', relief='sunken',
                               bg='black', fg='yellow',
                               font=('Courier', 100, 'bold'))
        self.nameBlueLabel = Label(parent, text='Blue', relief='sunken',
                                   bg='blue', fg='white',
                                   font=('Helvetica', 50, 'bold'))
        self.scoreRedLabel = Label(parent, text='000', relief='sunken',
                                   bg='black', fg='yellow',
                                   font=('Courier', 100, 'bold'))
        self.scoreBlueLabel = Label(parent, text='000', relief='sunken',
                                    bg='black', fg='yellow',
                                    font=('Courier', 100, 'bold'))
        self.buttonRed = Button(parent, text='+',
                                font=('Helvetica', 100, 'bold'),
                                command=self.increase_red_score)
        self.buttonTime = Button(parent, text='▶',
                                 font=('Helvetica', 100, 'bold'),
                                 command=self.toggle_time)
        self.buttonBlue = Button(parent, text='+',
                                 font=('Helvetica', 100, 'bold'),
                                 command=self.increase_blue_score)

        self.nameRedLabel.grid(row=0, column=0, sticky=NSEW)
        self.timeLabel.grid(row=0, column=1, sticky=NSEW, rowspan=2)
        self.nameBlueLabel.grid(row=0, column=2, sticky=NSEW)
        self.scoreRedLabel.grid(row=1, column=0, sticky=NSEW)
        self.scoreBlueLabel.grid(row=1, column=2, sticky=NSEW)
        self.buttonRed.grid(row=2, column=0, sticky=NSEW)
        self.buttonTime.grid(row=2, column=1, sticky=NSEW)
        self.buttonBlue.grid(row=2, column=2, sticky=NSEW)

        parent.rowconfigure(0, weight=2)
        parent.rowconfigure(1, weight=2)
        parent.rowconfigure(2, weight=1)
        parent.columnconfigure(0, weight=1)
        parent.columnconfigure(1, weight=3)
        parent.columnconfigure(2, weight=1)

    def time_string(self):
        return '{0:02.0f}:{1:02.0f}'.format(self.timeLeft//60,
                                            self.timeLeft % 60)

    def score_string(self, score):
        return '{0:03.0f}'.format(score)

    def toggle_time(self):
        if self.running:
            self.running = False
            self.buttonTime.config(text='▶')
            self.after_cancel(self.timer)
            self.timer = None
        else:
            self.running = True
            self.buttonTime.config(text='■')
            self.timer = self.after(1000, self.decrease_time)

    def increase_red_score(self):
        self.scoreRed += 1
        self.scoreRedLabel.config(text=self.score_string(self.scoreRed))

    def increase_blue_score(self):
        self.scoreBlue += 1
        self.scoreBlueLabel.config(text=self.score_string(self.scoreBlue))

    def decrease_time(self):
        self.timeLeft -= 1
        if self.timeLeft <= 0:
            self.toggle_time()
            self.timeLeft = ScoreBoard.DEFAULT_GAME_LENGTH
        else:
            self.timer = self.after(1000, self.decrease_time)
        self.timeLabel.config(text=self.time_string())


def toggle_time(board):
    board.toggle_time()


def increase_red_score(board):
    board.increase_red_score()


def increase_blue_score(board):
    board.increase_blue_score()


root = Tk()
root.title('RoboArena Score Board')
root.geometry("900x600+100+100")
board = ScoreBoard(root)
root.bind('<space>', lambda event: toggle_time(board))
root.bind('q', lambda event: increase_red_score(board))
root.bind('o', lambda event: increase_red_score(board))
root.bind('w', lambda event: increase_blue_score(board))
root.bind('p', lambda event: increase_blue_score(board))

mainloop()
