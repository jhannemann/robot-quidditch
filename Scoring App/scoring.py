from tkinter import *
from datetime import timedelta


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
                                command=self.increaseRedScore)
        self.buttonTime = Button(parent, text='▶',
                                 font=('Helvetica', 100, 'bold'),
                                 command=self.toggleTime)
        self.buttonBlue = Button(parent, text='+',
                                 font=('Helvetica', 100, 'bold'),
                                 command=self.increaseBlueScore)

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

    def timeString(self):
        return '{0:02.0f}:{1:02.0f}'.format(self.timeLeft//60,
                                            self.timeLeft % 60)

    def scoreString(self, score):
        return '{0:03.0f}'.format(score)

    def toggleTime(self):
        if self.running:
            self.running = False
            self.buttonTime.config(text='▶')
            self.after_cancel(self.timer)
            self.timer = None
        else:
            self.running = True
            self.buttonTime.config(text='■')
            self.timer = self.after(1000, self.decreaseTime)

    def increaseRedScore(self):
        self.scoreRed += 1
        self.scoreRedLabel.config(text=self.scoreString(self.scoreRed))

    def increaseBlueScore(self):
        self.scoreBlue += 1
        self.scoreBlueLabel.config(text=self.scoreString(self.scoreBlue))

    def decreaseTime(self):
        self.timeLeft -= 1
        if self.timeLeft <= 0:
            self.toggleTime()
            self.timeLeft = ScoreBoard.DEFAULT_GAME_LENGTH
        else:
            self.timer = self.after(1000, self.decreaseTime)
        self.timeLabel.config(text=self.timeString())


def spacePressed(board):
    board.toggleTime()


def aPressed(board):
    board.increaseRedScore()


def sPressed(board):
    board.increaseBlueScore()


root = Tk()
root.title('RoboArena Score Board')
root.geometry("900x600+100+100")
board = ScoreBoard(root)
root.bind('<space>', lambda event: spacePressed(board))
root.bind('a', lambda event: aPressed(board))
root.bind('s', lambda event: sPressed(board))

mainloop()
