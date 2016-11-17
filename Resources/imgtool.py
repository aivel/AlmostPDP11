from PIL import Image

im = Image.open("test.bmp")
pix = im.load()

EDGE = 160


def get_pixels(pic, w, h):
    pixels = []

    for y in range(h):
        for x in range(w):
            pixels.append(pic[x, y])

    return pixels


def get_pixel_components(pixels):
    components = []

    for pixel in pixels:
        components.extend(pixel)

    return components


def pack_component(component):
    return component >= EDGE


def pack_components(components):
    return [pack_component(component) for component in components]


def chunks(l, n=8):
    _chunks = []

    for i in range(0, len(l), n):
        _chunks.append(l[i:i + n])

    return _chunks


def get_byte(bits):
    bits = bits[::-1]
    bits = ''.join(['1' if bit else '0' for bit in bits])
    val = int('0b%s' % bits, 2)

    # for i in range(8):
    #     val = (val | bits[i])

    return val


def pack_colors(components):
    component_chunks = chunks(components)
    need_bits = 8 - len(component_chunks[-1])

    for i in range(need_bits):
        component_chunks[-1].append(0)

    return [get_byte(chunk) for chunk in component_chunks]


pixels = get_pixels(pix, *im.size)
components = get_pixel_components(pixels)
bitstream = pack_components(components)

print('Total bits: %s' % len(bitstream))

packed_colors = bytes(pack_colors(bitstream))

open('test.bin', 'wb').write(packed_colors)
