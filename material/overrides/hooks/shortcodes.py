# shortcodes.py

################################################################################################################################
# A solution inspired by Material for mkdocs' own "shortcodes.py" hook
# Implemented by Matthew

# Credits to: https://github.com/squidfunk/mkdocs-material/blob/master/material/overrides/hooks/shortcodes.py (MIT License)
################################################################################################################################


import os
import re
from typing import Any

from mkdocs.config.defaults import MkDocsConfig
from mkdocs.structure.files import File, Files
from mkdocs.structure.pages import Page


def on_page_markdown(
    markdown: str, *, page: Page, config: MkDocsConfig, files: Files
) -> str | Any:
    """
    shortcodes.py entrypoint.

    :param str markdown: the markdown
    :param Page page: the page
    :param MkDocsConfig config: the mkdocs.yml content (ignored)
    :param Files files: the files
    :raises RuntimeError: unknown shortcode
    :return str | Any:
    """
    
    # Replace callback
    def replace(match: re.Match):
        type, args = match.groups()
        args: str = args.strip()
        
        match type:
            case "version":
                return Badges.get_badge_for_version(args, page, files)
            
            case "experimental":
                return Badges.get_badge_for_experimental_feature(page, files)
            
            case "option":
                return get_linkable_option(args)
            
            case "setting":
                return get_linkable_setting(args)
            
            case "utility":
                return Badges.get_badge_for_utility(args, page, files)
            
            case "example":
                return Badges.get_badge_for_example_view_and_download(args)
            
            case "demo":
                return Badges.get_badge_for_demo_repository(args)

        # Otherwise, raise an error
        raise RuntimeError(f"Unknown shortcode: {type}")

    # Find and replace all external asset URLs in current page
    return re.sub(
        r"<!-- md:(\w+)(.*?) -->",
        replace, markdown, flags = re.I | re.M
    )


def get_linkable_option(type: str) -> str:
    """
    Creates a linkable option.

    :param str type:
    :return str: the option
    """
    
    _, *_, name = re.split(r"[.:]", type)
    return f"[`{name}`](#+{type}){{ #+{type} }}\n\n"


def get_linkable_setting(type: str) -> str:
    """
    Creates a linkable setting.

    :param str type:
    :return str: the setting
    """
    
    _, *_, name = re.split(r"[.*]", type)
    return f"`{name}` {{ #{type} }}\n\n[{type}]: #{type}\n\n"


def _resolve_path(path: str, page: Page, files: Files):
    path, anchor, *_ = f"{path}#".split("#")
    
    print(f"DEBUG: {path} ({anchor})")
    
    path = _resolve(files.get_file_from_path(path), page)
    return "#".join([path, anchor]) if anchor else path


def _resolve(file: File, page: Page):
    path = os.path.relpath(file.src_uri, page.file.src_uri)
    return os.pathsep.join(path.split(os.pathsep)[1:])


class Badges:
    @staticmethod
    def create_badge(icon: str, text: str = "", type: str = "") -> str:
        classes = f"mdx-badge mdx-badge--{type}" if type else "mdx-badge"
        return "".join([
            f"<span class=\"{classes}\">",
            *([f"<span class=\"mdx-badge__icon\">{icon}</span>"] if icon else []),
            *([f"<span class=\"mdx-badge__text\">{text}</span>"] if text else []),
            "</span>",
        ])
        
    @staticmethod
    def get_badge_for_version(text: str, page: Page, files: Files):
        spec = text
        path = f"changelog.md#{spec}"

        # Return badge
        icon = "material-tag-outline"
        href = _resolve_path("conventions.md#version", page, files)
        return Badges.create_badge(
            icon = f"[:{icon}:]({href} 'Minimum version')",
            text = f"[{text}]({_resolve_path(path, page, files)})" if spec else ""
        )

    @staticmethod
    def get_badge_for_utility(text: str, page: Page, files: Files):
        icon = "material-package-variant"
        href = _resolve_path("conventions.md#utility", page, files)
        return Badges.create_badge(
            icon = f"[:{icon}:]({href} 'Third-party utility')",
            text = text
        )

    @staticmethod
    def get_badge_for_example_view_and_download(text: str) -> str:
        def _get_badge_for_example_download() -> str:
            icon = "material-folder-download"
            href = f"https://oceanapocalypsestudios.org/rsml/examples/{text}.zip"
            return Badges.create_badge(
                icon = f"[:{icon}:]({href} 'Download example files')",
                text = f"[`.zip`]({href})",
                type = "right"
            )
        
        def _get_badge_for_example_view() -> str:
            icon = "material-folder-eye"
            href = f"https://oceanapocalypsestudios.org/rsml/examples/{text}/"
            return Badges.create_badge(
                icon = f"[:{icon}:]({href} 'View example')",
                type = "right"
            )
        
        return "\n".join([
            _get_badge_for_example_download(),
            _get_badge_for_example_view()
        ])
    
    @staticmethod
    def get_badge_for_demo_repository(text: str):
        icon = "material-github"
        href = f"https://github.com/OceanApocalypseStudios/{text}"
        return Badges.create_badge(
            icon = f"[:{icon}:]({href} 'Demo repository')",
            text = text,
            type = "right"
        )

    @staticmethod
    def get_badge_for_experimental_feature(page: Page, files: Files):
        icon = "material-flask-outline"
        href = _resolve_path("conventions.md#experimental", page, files)
        return Badges.create_badge(
            icon = f"[:{icon}:]({href} 'Experimental')"
        )
