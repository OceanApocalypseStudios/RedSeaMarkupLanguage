import sys
import os
import subprocess
import random

# [*] colorss
ASCII_COLOR: str = random.choice((f"\033[{random.randint(30, 37)}m",
								  f"\033[{random.randint(90, 97)}m"))
RED = "\033[31m"
GREEN = "\033[32m"
YELLOW = "\033[33m"
BLUE = "\033[34m"
MAGENTA = "\033[35m"
CYAN = "\033[36m"
RESET = "\033[0m"

STATUS_COLORS: dict[str, str] = {
	"Yes.": GREEN,
	"No.": RED,
	"Unsure.": YELLOW
}

output_format: str = "HTML"
emacs_on_path: str = "Unsure."


def is_emacs_on_path():
	process: subprocess.CompletedProcess | None = None

	try:
		process = subprocess.run(["emacs", "--version"], capture_output=True, text=True)

	except Exception:
		return False

	if process is None:
		return False

	if process.returncode != 0:
		return False

	return True


def show_build_screen():
	global output_format, emacs_on_path

	os.system('cls' if sys.platform == 'win32' else 'clear')
	print(f"""{ASCII_COLOR}    _      ______  _____ ___  ___ _        ______                         _    
 /\| |/\   | ___ \/  ___||  \/  || |       |  _  \                     /\| |/\ 
 \ ` ' /   | |_/ /\ `--. | .  . || |       | | | |  ___    ___  ___    \ ` ' / 
|_     _|  |    /  `--. \| |\/| || |       | | | | / _ \  / __|/ __|  |_     _|
 / , . \   | |\ \ /\__/ /| |  | || |____ _ | |/ / | (_) || (__ \__ \   / , . \ 
 \/|_|\/   \_| \_|\____/ \_|  |_/\_____/(_)|___/   \___/  \___||___/   \/|_|\/ 
                                                                               
                                                                               
Copyright (c) 2025 OceanApocalypseStudios{RESET}
=========================================
{MAGENTA}RSML stands for Red Sea Markup Language.{RESET}
It is made by {BLUE}OceanApocalypseStudios.{RESET}
=========================================
{YELLOW}Output format set to:{RESET} {output_format}
Emacs on PATH? {STATUS_COLORS[emacs_on_path]}{emacs_on_path}{RESET}
=========================================
{CYAN}[ 1 ]{RESET} Build Documentation with format OUTPUT_FORMAT
{CYAN}[ 2 ]{RESET} Check if Emacs is on PATH
{CYAN}[ 3 ]{RESET} Set OUTPUT_FORMAT to HTML
{CYAN}[ 4 ]{RESET} Set OUTPUT_FORMAT to PDF (requires LaTeX)
{CYAN}[ 5 ]{RESET} Set OUTPUT_FORMAT to Markdown
{RED}[...]{RESET} Exit
""")
	
	action: str = input(f"{CYAN}Your input:{RESET} ")

	if not action.isdigit():
		print(f"{RED}Aborting...{RESET}")
		sys.exit(1) # wrong option

	if action not in ('1', '2', '3', '4', '5'):
		print(f"{RED}Aborting...{RESET}")
		sys.exit(1)

	if action == "3":
		output_format = "HTML"
	
	if action == "4":
		output_format = "PDF"

	if action == "5":
		output_format = "Markdown"

	if action == "2":
		if is_emacs_on_path():
			emacs_on_path = "Yes."

		else:
			emacs_on_path = "No."

	if action == "1":
		try:
			subprocess.run([f"{'./' if sys.platform != 'win32' else ''}RSML.Docs{'.exe' if sys.platform == 'win32' else ''}", output_format.lower()], text=True)

		except Exception:
			print("Error! Failed to load RSML.Docs. Are you sure you're running this script in the same folder as the executable?\nAborting...")
			sys.exit(2)

	show_build_screen()


if __name__ == '__main__':
	print(f"{MAGENTA}Requirements:{RESET}\n {GREEN}[x] Emacs\n [x] RSML.Docs in the same directory\n{RESET}")
	input("Press any key to continue...")
	show_build_screen()
