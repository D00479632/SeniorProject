<lane orientation="vertical"
      horizontal-content-alignment="middle"
      opacity="1">
    <banner background={@Mods/StardewUI/Sprites/BannerBackground}
            background-border-thickness="48,0"
            padding="12"
            text="Get your random number" />
    <frame layout="500px content"
           margin="0,8,0,0"
           padding="32,24"
           background={@Mods/StardewUI/Sprites/ControlBorderUncolored}>
        <lane layout="stretch content" orientation="vertical">
            <form-row title="Enter text to get a random number:">
                <textinput layout="250px 48px"
                          margin="-6, 0, 0, 0"
                          max-length="100"
                          text={Text} />
            </form-row>
            <form-row title="Result:">
                <label layout="250px content"
                       margin="0,8"
                       text={RandomNumberText}
                       shadow-alpha="0.8"
                       shadow-color="#4448"
                       shadow-offset="-2, 2" />
            </form-row>
        </lane>
    </frame>
</lane>

<template name="form-row">
    <lane layout="stretch content"
          margin="16,4"
          vertical-content-alignment="middle">
        <label layout="200px content"
               margin="0,8"
               text={&title}
               shadow-alpha="0.8"
               shadow-color="#4448"
               shadow-offset="-2, 2" />
        <outlet />
    </lane>
</template> 