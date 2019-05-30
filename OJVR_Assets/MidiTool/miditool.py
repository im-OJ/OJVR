import json;
from mido import MidiFile;
from collections import defaultdict;

import mido;

# paths of txt config files
FILE_INPUT_PATH = 'config/input_path.txt';
FILE_OUTPUT_PATH = 'config/output_path.txt';
FILE_BPM = 'config/bpm.txt';

############ CLASSES ############

class Note:
    def __init__(self, note, velocity=64, time=None, duration=None):
        self.time = time;
        self.duration = duration;
        self.note = note;
        self.velocity = velocity;
        self.complete = True;

    def add_note_on_time(self,time):
        self.time = time;

    def add_note_off_time(self,time):
        self.duration = time - self.time;

    def is_complete(self):
        self.complete = True;
        if self.time is None:
            self.complete = False;
        if self.duration is None:
            self.complete = False;
        return self.complete;

    def to_dict(self):
        dict = {"time": self.time, "duration": self.duration, "velocity": self.velocity};
        return dict;

###### GENERAL USE FUNCTIONS #####

# gets path from path txt files
def get_txt(path):
    with open(path,'r') as file:
        pathtxt = file.read();
    return pathtxt;

def parse_midi(mid):
    total_time = 0;
    note_tracks = dict();
    for msg in mid:
        if not msg.is_meta:
            type = msg.type;
            note = get_note_str(msg.note);
            velocity = msg.velocity;
            time = (60/bpm)*(msg.time*2);
            if type == "note_on":
                if note not in note_tracks:
                    note_tracks[note] = [];
                note_tracks[note].append(Note(note, velocity))
                note_tracks[note][-1].add_note_on_time(total_time+time)
            if type == "note_off":
               note_tracks[note][-1].add_note_off_time(total_time+time);
            total_time = total_time + time;
    return note_tracks;

def note_tracks_to_json(tracks):
    output = defaultdict(list);
    for key in tracks:
        for note in tracks[key]:
            output[key].append(note.to_dict());
    return json.dumps(output);

# returns midi note string from midi note number.
# from: https://pythonhosted.org/audiolazy/_modules/audiolazy/lazy_midi.html
def get_note_str(midi_number, sharp=True):
    MIDI_A4 = 69  # MIDI Pitch number
    num = midi_number - (MIDI_A4 - 4 * 12 - 9)
    note = (num + .5) % 12 - .5
    rnote = int(round(note))
    error = note - rnote
    octave = str(int(round((num - note) / 12.))-1)
    if sharp:
        names = ["C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B"]
    else:
        names = ["C", "Db", "D", "Eb", "E", "F", "Gb", "G", "Ab", "A", "Bb", "B"]
    names = names[rnote] + octave
    if abs(error) < 1e-4:
        return names
    else:
        err_sig = "+" if error > 0 else "-"
        err_str = err_sig + str(round(100 * abs(error), 2)) + "%"
        return names + err_str

def save_to_output(string):
    with open(output_path + '\output.json', 'w') as outfile:
        outfile.write(string)

########### ALGORITHM ###########

input_path = get_txt(FILE_INPUT_PATH);
output_path = get_txt(FILE_OUTPUT_PATH);
bpm = int(get_txt(FILE_BPM));
tempo = mido.bpm2tempo(bpm);

print("Welcome to MidiTool \n  ****input_path.txt must point to a midi file (eg. 'C:\mymidi.midi') **** \n ****output_path.txt must point to a directory (eg. 'C:\myfolder')****\n Remember to update bpm.txt");


while True:
    print("===================================");
    input_path = get_txt(FILE_INPUT_PATH);
    output_path = get_txt(FILE_OUTPUT_PATH);
    bpm = int(get_txt(FILE_BPM));
    tempo = mido.bpm2tempo(bpm);
    print("input file path: " + input_path);
    print("output file path: " + output_path);
    print("bpm: " + str(bpm));

    input("Press Enter to continue...");

    mid = MidiFile(input_path);
    ticks_per_beat = mid.ticks_per_beat;
    print("Got midi data:");

    print("parsing midi");
    note_tracks = parse_midi(mid);

    print("converting to json");
    json_output = note_tracks_to_json(note_tracks);
    print("got output:");
    print(json_output);
    save_to_output(json_output);
    input("Press Enter to refresh");
