import sys

def process_file(input_file):
    output_file = input_file.replace('.txt', '_usernames.txt')
    
    with open(input_file, 'r') as infile, open(output_file, 'w') as outfile:
        for line in infile:
            # Split the line by the first '|' and then extract the username part
            parts = line.split('|')
            if parts:
                # Extract username from "Username: <username>"
                username = parts[0].replace('Username:', '').strip()
                outfile.write(f"{username}\n")  # Write the username to the output file
    
    print(f"Filtered file saved as {output_file}")

if __name__ == "__main__":
    # Check if the script was provided with a file through drag and drop
    if len(sys.argv) > 1:
        process_file(sys.argv[1])
    else:
        print("Please drag and drop a text file onto this script.")
