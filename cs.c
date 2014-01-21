/* Capstone Disassembler Engine */
/* By Nguyen Anh Quynh <aquynh@gmail.com>, 2013> */

#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <capstone.h>

<<<<<<< HEAD
#include "cs_priv.h"

#include "MCRegisterInfo.h"

#include "arch/X86/X86Disassembler.h"
#include "arch/X86/X86InstPrinter.h"
#include "arch/X86/mapping.h"
=======
#include "utils.h"
#include "MCRegisterInfo.h"
>>>>>>> upstream/master

#include "arch/ARM/ARMDisassembler.h"
#include "arch/ARM/ARMInstPrinter.h"
#include "arch/ARM/mapping.h"

<<<<<<< HEAD
#include "arch/Mips/MipsDisassembler.h"
#include "arch/Mips/MipsInstPrinter.h"
#include "arch/Mips/mapping.h"

#include "arch/AArch64/AArch64Disassembler.h"
#include "arch/AArch64/AArch64InstPrinter.h"
#include "arch/AArch64/mapping.h"
=======
extern void ARM_enable(void);
extern void AArch64_enable(void);
extern void Mips_enable(void);
extern void X86_enable(void);
extern void PPC_enable(void);

static void archs_enable(void)
{
	static bool initialized = false;

	if (initialized)
		return;

#ifdef CAPSTONE_HAS_ARM
	ARM_enable();
#endif
#ifdef CAPSTONE_HAS_ARM64
	AArch64_enable();
#endif
#ifdef CAPSTONE_HAS_MIPS
	Mips_enable();
#endif
#ifdef CAPSTONE_HAS_X86
	X86_enable();
#endif
#ifdef CAPSTONE_HAS_POWERPC
	PPC_enable();
#endif

	initialized = true;
}

unsigned int all_arch = 0;

#ifdef USE_SYS_DYN_MEM
cs_malloc_t cs_mem_malloc = malloc;
cs_calloc_t cs_mem_calloc = calloc;
cs_realloc_t cs_mem_realloc = realloc;
cs_free_t cs_mem_free = free;
cs_vsnprintf_t cs_vsnprintf = vsnprintf;
#else
cs_malloc_t cs_mem_malloc = NULL;
cs_calloc_t cs_mem_calloc = NULL;
cs_realloc_t cs_mem_realloc = NULL;
cs_free_t cs_mem_free = NULL;
cs_vsnprintf_t cs_vsnprintf = NULL;
#endif
>>>>>>> upstream/master

#include "utils.h"


<<<<<<< HEAD
void cs_version(int *major, int *minor)
=======
bool cs_support(int arch)
>>>>>>> upstream/master
{
	*major = CS_API_MAJOR;
	*minor = CS_API_MINOR;
}

cs_err cs_errno(csh handle)
{
	if (!handle)
		return CS_ERR_CSH;

	cs_struct *ud = (cs_struct *)(uintptr_t)handle;

	return ud->errnum;
}

<<<<<<< HEAD
=======
const char *cs_strerror(cs_err code)
{
	switch(code) {
		default:
			return "Unknown error code";
		case CS_ERR_OK:
			return "OK (CS_ERR_OK)";
		case CS_ERR_MEM:
			return "Out of memory (CS_ERR_MEM)";
		case CS_ERR_ARCH:
			return "Invalid architecture (CS_ERR_ARCH)";
		case CS_ERR_HANDLE:
			return "Invalid handle (CS_ERR_HANDLE)";
		case CS_ERR_CSH:
			return "Invalid csh (CS_ERR_CSH)";
		case CS_ERR_MODE:
			return "Invalid mode (CS_ERR_MODE)";
		case CS_ERR_OPTION:
			return "Invalid option (CS_ERR_OPTION)";
		case CS_ERR_DETAIL:
			return "Details are unavailable (CS_ERR_DETAIL)";
		case CS_ERR_MEMSETUP:
			return "Dynamic memory management uninitialized (CS_ERR_MEMSETUP)";
	}
}

>>>>>>> upstream/master
cs_err cs_open(cs_arch arch, cs_mode mode, csh *handle)
{
	if (!cs_mem_malloc || !cs_mem_calloc || !cs_mem_realloc || !cs_mem_free || !cs_vsnprintf)
		// Error: before cs_open(), dynamic memory management must be initialized
		// with cs_option(CS_OPT_MEM)
		return CS_ERR_MEMSETUP;

	archs_enable();

	if (arch < CS_ARCH_MAX && arch_init[arch]) {
		cs_struct *ud;

		ud = cs_mem_calloc(1, sizeof(*ud));
		if (!ud) {
			// memory insufficient
			return CS_ERR_MEM;
		}

<<<<<<< HEAD
	ud->errnum = CS_ERR_OK;
	ud->arch = arch;
	ud->mode = mode;
	ud->big_endian = mode & CS_MODE_BIG_ENDIAN;
	ud->reg_name = NULL;
	ud->detail = CS_OPT_ON;	// by default break instruction into details

	switch (ud->arch) {
		case CS_ARCH_X86:
			// by default, we use Intel syntax
			ud->printer = X86_Intel_printInst;
			ud->printer_info = NULL;
			ud->disasm = X86_getInstruction;
			ud->reg_name = X86_reg_name;
			ud->insn_id = X86_get_insn_id;
			ud->insn_name = X86_insn_name;
			ud->post_printer = X86_post_printer;
			break;
		case CS_ARCH_ARM: {
					MCRegisterInfo *mri = malloc(sizeof(*mri));

					ARM_init(mri);

					ud->printer = ARM_printInst;
					ud->printer_info = mri;
					ud->reg_name = ARM_reg_name;
					ud->insn_id = ARM_get_insn_id;
					ud->insn_name = ARM_insn_name;
					ud->post_printer = ARM_post_printer;

					if (ud->mode & CS_MODE_THUMB)
						ud->disasm = Thumb_getInstruction;
					else
						ud->disasm = ARM_getInstruction;
					break;
				}
		case CS_ARCH_MIPS: {
				   MCRegisterInfo *mri = malloc(sizeof(*mri));

				   Mips_init(mri);
				   ud->printer = Mips_printInst;
				   ud->printer_info = mri;
				   ud->getinsn_info = mri;
				   ud->reg_name = Mips_reg_name;
				   ud->insn_id = Mips_get_insn_id;
				   ud->insn_name = Mips_insn_name;

				   if (ud->mode & CS_MODE_32)
					   ud->disasm = Mips_getInstruction;
				   else
					   ud->disasm = Mips64_getInstruction;

				   break;
			}
		case CS_ARCH_ARM64: {
					MCRegisterInfo *mri = malloc(sizeof(*mri));

					AArch64_init(mri);
					ud->printer = AArch64_printInst;
					ud->printer_info = mri;
					ud->getinsn_info = mri;
					ud->disasm = AArch64_getInstruction;
					ud->reg_name = AArch64_reg_name;
					ud->insn_id = AArch64_get_insn_id;
					ud->insn_name = AArch64_insn_name;
					ud->post_printer = AArch64_post_printer;
					break;
			}
		default:	// unsupported architecture
			free(ud);
			return CS_ERR_ARCH;
=======
		ud->errnum = CS_ERR_OK;
		ud->arch = arch;
		ud->mode = mode;
		ud->big_endian = mode & CS_MODE_BIG_ENDIAN;
		// by default, do not break instruction into details
		ud->detail = CS_OPT_OFF;

		cs_err err = arch_init[ud->arch](ud);
		if (err) {
			cs_mem_free(ud);
			*handle = 0;
			return err;
		}

		*handle = (uintptr_t)ud;

		return CS_ERR_OK;
	} else {
		*handle = 0;
		return CS_ERR_ARCH;
>>>>>>> upstream/master
	}
}

cs_err cs_close(csh handle)
{
	if (!handle)
		return CS_ERR_CSH;

	cs_struct *ud = (cs_struct *)(uintptr_t)handle;

	switch (ud->arch) {
		case CS_ARCH_X86:
			break;
		case CS_ARCH_ARM:
		case CS_ARCH_MIPS:
		case CS_ARCH_ARM64:
<<<<<<< HEAD
			free(ud->printer_info);
=======
		case CS_ARCH_PPC:
			cs_mem_free(ud->printer_info);
>>>>>>> upstream/master
			break;
		default:	// unsupported architecture
			return CS_ERR_HANDLE;
	}

<<<<<<< HEAD
=======
	// arch_destroy[ud->arch](ud);

	cs_mem_free(ud->insn_cache);
>>>>>>> upstream/master
	memset(ud, 0, sizeof(*ud));
	cs_mem_free(ud);

	return CS_ERR_OK;
}

#define MIN(x, y) ((x) < (y) ? (x) : (y))

// fill insn with mnemonic & operands info
static void fill_insn(cs_struct *handle, cs_insn *insn, char *buffer, MCInst *mci,
		PostPrinter_t printer, const uint8_t *code)
{
	if (handle->detail) {
		memcpy(insn, &mci->pub_insn, sizeof(*insn));

		// fill the instruction bytes
		memcpy(insn->bytes, code, MIN(sizeof(insn->bytes), insn->size));

	} else {
		insn->address = mci->address;
		insn->size = mci->insn_size;
	}

	// map internal instruction opcode to public insn ID
	if (handle->insn_id)
		handle->insn_id(handle, insn, MCInst_getOpcode(mci));

	// alias instruction might have ID saved in OpcodePub
	if (MCInst_getOpcodePub(mci))
		insn->id = MCInst_getOpcodePub(mci);

	// post printer handles some corner cases (hacky)
	if (printer)
		printer((csh)handle, insn, buffer);

	// fill in mnemonic & operands
	// find first space or tab
	char *sp = buffer;
	for (sp = buffer; *sp; sp++)
		if (*sp == ' '||*sp == '\t')
			break;
	if (*sp) {
		*sp = '\0';
		// find the next non-space char
		sp++;
		for (; ((*sp == ' ') || (*sp == '\t')); sp++);
		strncpy(insn->op_str, sp, sizeof(insn->op_str) - 1);
		insn->op_str[sizeof(insn->op_str) - 1] = '\0';
	} else
		insn->op_str[0] = '\0';

	strncpy(insn->mnemonic, buffer, sizeof(insn->mnemonic) - 1);
	insn->mnemonic[sizeof(insn->mnemonic) - 1] = '\0';
}

cs_err cs_option(csh ud, cs_opt_type type, size_t value)
{
	// cs_option() can be called with NULL handle just for CS_OPT_MEM
	// This is supposed to be executed before all other APIs (even cs_open())
	if (type == CS_OPT_MEM) {
		cs_opt_mem *mem = (cs_opt_mem *)value;

		cs_mem_malloc = mem->malloc;
		cs_mem_calloc = mem->calloc;
		cs_mem_realloc = mem->realloc;
		cs_mem_free = mem->free;
		cs_vsnprintf = mem->vsnprintf;

		return CS_ERR_OK;
	}

	cs_struct *handle = (cs_struct *)(uintptr_t)ud;
	if (!handle)
		return CS_ERR_CSH;

	switch(type) {
		default:
			break;
		case CS_OPT_DETAIL:
			handle->detail = value;
			return CS_ERR_OK;
	}

	// only selected archs care about CS_OPT_SYNTAX
	switch (handle->arch) {
		default:
			handle->errnum = CS_ERR_OPTION;
			return CS_ERR_OPTION;

		case CS_ARCH_X86:
			if (type & CS_OPT_SYNTAX) {
				switch(value) {
					default:
						handle->errnum = CS_ERR_OPTION;
						return CS_ERR_OPTION;

					case CS_OPT_SYNTAX_INTEL:
						handle->printer = X86_Intel_printInst;
						break;

					case CS_OPT_SYNTAX_ATT:
						handle->printer = X86_ATT_printInst;
						break;
				}
			} else {
				handle->errnum = CS_ERR_OPTION;
				return CS_ERR_OPTION;
			}
			break;
	}

	return CS_ERR_OK;
}

size_t cs_disasm(csh ud, const uint8_t *buffer, size_t size, uint64_t offset, size_t count, cs_insn *insn)
{
	cs_struct *handle = (cs_struct *)(uintptr_t)ud;
	MCInst mci;
	uint16_t insn_size;
	size_t c = 0;

	if (!handle) {
		// FIXME: handle this case?
		// handle->errnum = CS_ERR_HANDLE;
		return 0;
	}

	handle->errnum = CS_ERR_OK;
	memset(insn, 0, count * sizeof(*insn));

	while (size > 0) {
		MCInst_Init(&mci);	
		mci.detail = handle->detail;
		mci.mode = handle->mode;

		bool r = handle->disasm(ud, buffer, size, &mci, &insn_size, offset, handle->getinsn_info);
		if (r) {
			SStream ss;
			SStream_Init(&ss);

			// relative branches need to know the address & size of current insn
			mci.insn_size = insn_size;
			mci.address = offset;

			if (handle->detail) {
				// save all the information for non-detailed mode
				mci.pub_insn.address = offset;
				mci.pub_insn.size = insn_size;
			}

			handle->printer(&mci, &ss, handle->printer_info);

			fill_insn(handle, insn, ss.buffer, &mci, handle->post_printer, buffer);

			c++;
			insn++;
			buffer += insn_size;
			size -= insn_size;
			offset += insn_size;

			if (c == count)
				return c;
		} else
			// face a broken instruction? then we stop here
			return c;
	}

	return c;
}

// get previous instruction, which can be in the cache, or in total buffer
static cs_insn *get_prev_insn(cs_insn *cache, unsigned int f, void *total, size_t total_size)
{
	if (f == 0) {
		if (total == NULL)
			return NULL;
		// get the trailing insn from total buffer
		return (cs_insn *)(total + total_size - sizeof(cs_insn));
	} else
		return &cache[f - 1];
}

// dynamicly allocate memory to contain disasm insn
// NOTE: caller must free() the allocated memory itself to avoid memory leaking
size_t cs_disasm_dyn(csh ud, const uint8_t *buffer, size_t size, uint64_t offset, size_t count, cs_insn **insn)
{
	cs_struct *handle = (cs_struct *)(uintptr_t)ud;
	MCInst mci;
	uint16_t insn_size;
	size_t c = 0;
	unsigned int f = 0;
	cs_insn insn_cache[64];
	void *total = NULL;
	size_t total_size = 0;

	if (!handle) {
		// FIXME: how to handle this case:
		// handle->errnum = CS_ERR_HANDLE;
		return 0;
	}

	handle->errnum = CS_ERR_OK;

	memset(insn_cache, 0, sizeof(insn_cache));

	while (size > 0) {
		MCInst_Init(&mci);	
		mci.detail = handle->detail;
		mci.mode = handle->mode;

		bool r = handle->disasm(ud, buffer, size, &mci, &insn_size, offset, handle->getinsn_info);
		if (r) {
			SStream ss;
			SStream_Init(&ss);

			// relative branches need to know the address & size of current insn
			mci.insn_size = insn_size;
			mci.address = offset;

			if (handle->detail) {
				// save all the information for non-detailed mode
<<<<<<< HEAD
				mci.pub_insn.address = offset;
				mci.pub_insn.size = insn_size;
=======
				mci.flat_insn.address = offset;
				mci.flat_insn.size = insn_size;
				// allocate memory for @detail pointer
				insn_cache[f].detail = cs_mem_calloc(1, sizeof(cs_detail));
>>>>>>> upstream/master
			}

			handle->printer(&mci, &ss, handle->printer_info);

			fill_insn(handle, &insn_cache[f], ss.buffer, &mci, handle->post_printer, buffer);

			if (!handle->check_combine || !handle->check_combine(handle, &insn_cache[f])) {
				f++;

				if (f == ARR_SIZE(insn_cache)) {
					// resize total to contain newly disasm insns
					total_size += sizeof(insn_cache);
					void *tmp = cs_mem_realloc(total, total_size);
					if (tmp == NULL) {	// insufficient memory
						cs_mem_free(total);
						handle->errnum = CS_ERR_MEM;
						return 0;
					}

					total = tmp;
					memcpy(total + total_size - sizeof(insn_cache), insn_cache, sizeof(insn_cache));
					// reset f back to 0
					f = 0;
				}

				c++;
			} else {
				// combine this instruction with previous prefix instruction
				cs_insn *prev = get_prev_insn(insn_cache, f, total, total_size);
				handle->combine(handle, &insn_cache[f], prev);
			}

			buffer += insn_size;
			size -= insn_size;
			offset += insn_size;

			if (count > 0 && c == count)
				break;
		} else	{
			// encounter a broken instruction
			// XXX: TODO: JOXEAN continue here
			break;
		}
	}

	if (f) {
		// resize total to contain newly disasm insns
		void *tmp = cs_mem_realloc(total, total_size + f * sizeof(insn_cache[0]));
		if (tmp == NULL) {	// insufficient memory
			cs_mem_free(total);
			handle->errnum = CS_ERR_MEM;
			return 0;
		}

		total = tmp;
		memcpy(total + total_size, insn_cache, f * sizeof(insn_cache[0]));
	}

	*insn = total;

	return c;
}

void cs_free(void *m)
{
<<<<<<< HEAD
	free(m);
=======
	size_t i;

	// free all detail pointers
	for (i = 0; i < count; i++)
		cs_mem_free(insn[i].detail);

	// then free pointer to cs_insn array
	cs_mem_free(insn);
>>>>>>> upstream/master
}

// return friendly name of regiser in a string
const char *cs_reg_name(csh ud, unsigned int reg)
{
	cs_struct *handle = (cs_struct *)(uintptr_t)ud;

	if (!handle || handle->reg_name == NULL) {
		return NULL;
	}

	return handle->reg_name(ud, reg);
}

const char *cs_insn_name(csh ud, unsigned int insn)
{
	cs_struct *handle = (cs_struct *)(uintptr_t)ud;

	if (!handle || handle->insn_name == NULL) {
		return NULL;
	}

	return handle->insn_name(ud, insn);
}

static bool arr_exist(unsigned int *arr, int max, unsigned int id)
{
	int i;

	for (i = 0; i < max; i++) {
		if (arr[i] == id)
			return true;
	}

	return false;
}

bool cs_insn_group(csh handle, cs_insn *insn, unsigned int group_id)
{
<<<<<<< HEAD
	if (!handle)
=======
	if (!ud)
		return false;

	cs_struct *handle = (cs_struct *)(uintptr_t)ud;
	if (!handle->detail) {
		handle->errnum = CS_ERR_DETAIL;
>>>>>>> upstream/master
		return false;

	return arr_exist(insn->groups, insn->groups_count, group_id);
}

bool cs_reg_read(csh handle, cs_insn *insn, unsigned int reg_id)
{
<<<<<<< HEAD
	if (!handle)
=======
	if (!ud)
		return false;

	cs_struct *handle = (cs_struct *)(uintptr_t)ud;
	if (!handle->detail) {
		handle->errnum = CS_ERR_DETAIL;
>>>>>>> upstream/master
		return false;

	return arr_exist(insn->regs_read, insn->regs_read_count, reg_id);
}

bool cs_reg_write(csh handle, cs_insn *insn, unsigned int reg_id)
{
<<<<<<< HEAD
	if (!handle)
=======
	if (!ud)
		return false;

	cs_struct *handle = (cs_struct *)(uintptr_t)ud;
	if (!handle->detail) {
		handle->errnum = CS_ERR_DETAIL;
>>>>>>> upstream/master
		return false;

	return arr_exist(insn->regs_write, insn->regs_write_count, reg_id);
}

int cs_op_count(csh ud, cs_insn *insn, unsigned int op_type)
{
	if (!ud)
		return -1;

	cs_struct *handle = (cs_struct *)(uintptr_t)ud;
	if (!handle->detail) {
		handle->errnum = CS_ERR_DETAIL;
		return -1;
	}

	unsigned int count = 0, i;

	handle->errnum = CS_ERR_OK;

	switch (handle->arch) {
		default:
			handle->errnum = CS_ERR_HANDLE;
			return -1;
		case CS_ARCH_ARM:
			for (i = 0; i < insn->arm.op_count; i++)
				if (insn->arm.operands[i].type == op_type)
					count++;
			break;
		case CS_ARCH_ARM64:
			for (i = 0; i < insn->arm64.op_count; i++)
				if (insn->arm64.operands[i].type == op_type)
					count++;
			break;
		case CS_ARCH_X86:
			for (i = 0; i < insn->x86.op_count; i++)
				if (insn->x86.operands[i].type == op_type)
					count++;
			break;
		case CS_ARCH_MIPS:
			for (i = 0; i < insn->mips.op_count; i++)
				if (insn->mips.operands[i].type == op_type)
					count++;
			break;
	}

	return count;
}

int cs_op_index(csh ud, cs_insn *insn, unsigned int op_type,
		unsigned int post)
{
	if (!ud)
		return -1;

	cs_struct *handle = (cs_struct *)(uintptr_t)ud;
	if (!handle->detail) {
		handle->errnum = CS_ERR_DETAIL;
		return -1;
	}

	unsigned int count = 0, i;

	handle->errnum = CS_ERR_OK;

	switch (handle->arch) {
		default:
			handle->errnum = CS_ERR_HANDLE;
			return -1;
		case CS_ARCH_ARM:
			for (i = 0; i < insn->arm.op_count; i++) {
				if (insn->arm.operands[i].type == op_type)
					count++;
				if (count == post)
					return i;
			}
			break;
		case CS_ARCH_ARM64:
			for (i = 0; i < insn->arm64.op_count; i++) {
				if (insn->arm64.operands[i].type == op_type)
					count++;
				if (count == post)
					return i;
			}
			break;
		case CS_ARCH_X86:
			for (i = 0; i < insn->x86.op_count; i++) {
				if (insn->x86.operands[i].type == op_type)
					count++;
				if (count == post)
					return i;
			}
			break;
		case CS_ARCH_MIPS:
			for (i = 0; i < insn->mips.op_count; i++) {
				if (insn->mips.operands[i].type == op_type)
					count++;
				if (count == post)
					return i;
			}
			break;
	}

	return -1;
}
