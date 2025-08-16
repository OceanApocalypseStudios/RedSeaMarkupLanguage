import random


def generate_complex_content(lines: int) -> str:
    content: str = ''

    for i in range(lines):
        v = random.randint(0, 5)

        if v == 0:
            content += f'-> windows "value{i}"\n'

        elif v == 1:
            content += f'@SpecialAction arg{i}\n'

        elif v == 2:
            content += f'!> windows x64 "value{i}"\n'

        elif v == 3:
            content += f'-> windows {i} x64 "value{i}"\n'

        elif v == 4:
            content += f'# Comment\n'

        else:
            content += '    # Comment\n'

    return content


def generate_content(lines: int) -> str:
    content: str = ''

    for i in range(lines):
        if i % 5 == 0:
            content += f'-> windows "value{i}"\n'
            continue

        content += f'# Comment {i}\n'

    return content


if __name__ == '__main__':
    lines = int(input("Number of lines: "))
    is_complex_raw = input("Generate complex data? ")

    is_complex = is_complex_raw == 'y'

    if is_complex:
        print(generate_complex_content(lines))
        exit()

    print(generate_content(lines))
    exit()
